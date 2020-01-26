using System;
using System.Collections.Generic;
using Infrastructure.EventSourcing;
using Payments.Contracts.Events;

namespace Payments
{
    public class Payment : EventSourced, IMementoOriginator
    {
        private Guid _chargeId;
        private PaymentAmount _amount;
        private CreditCardInfo _cardInfo;
        private PaymentState _paymentState;

        protected Payment(Guid id) : base(id)
        {
            // You'll notice that the public methods don't update any state themselves. Instead, they add events and
            // the handlers update the state. This allows the state of this class to be re-built from events.

            Handles<PaymentInitiated>(OnPaymentInitiated);
            Handles<PaymentCompleted>(OnPaymentCompleted);
            Handles<PaymentRejected>(OnPaymentRejected);
        }

        public Payment(Guid id, IEnumerable<IVersionedEvent> history) : this(id)
        {
            LoadFrom(history);
        }

        public Payment(Guid id, IMemento memento, IEnumerable<IVersionedEvent> history) : this(id)
        {
            var state = (Memento)memento;

            Version = state.Version;

            _chargeId = state.ChargeId;
            _amount = state.Amount;
            _cardInfo = state.CardInfo;
            _paymentState = state.PaymentState;
            
            LoadFrom(history);
        }

        public Payment(Guid chargeId, PaymentAmount amount, CreditCardInfo cardInfo) : this(chargeId)
        {
            if (amount is null) throw new ArgumentNullException(nameof(amount));
            if (cardInfo is null) throw new ArgumentNullException(nameof(cardInfo));

            Update(new PaymentInitiated 
            {
                ChargeId = chargeId,
                Amount = amount.Value,
                Currency = amount.Currency,
                CardNumber = cardInfo.CardNumber,
                Cvv = cardInfo.Cvv,
                ExpiryMonth = cardInfo.ExpiryMonth,
                ExpiryYear = cardInfo.ExpiryYear
            });
        }

        public void Confirm()
        {
            Update(new PaymentCompleted
            {
                ChargeId = _chargeId
            });
        }

        public void Reject()
        {
            Update(new PaymentRejected
            {
                ChargeId = _chargeId
            });
        }

        private void OnPaymentInitiated(PaymentInitiated e)
        {
            _paymentState = PaymentState.Pending;

            _amount = new PaymentAmount
            {
                Value = e.Amount,
                Currency = e.Currency
            };

            _cardInfo = new CreditCardInfo
            {
                CardNumber = e.CardNumber,
                Cvv = e.Cvv,
                ExpiryMonth = e.ExpiryMonth,
                ExpiryYear = e.ExpiryYear
            };
        }

        private void OnPaymentCompleted(PaymentCompleted e)
        {
            _paymentState = PaymentState.Success;
        }

        private void OnPaymentRejected(PaymentRejected e)
        {
            _paymentState = PaymentState.Failed;
        }

        // Saves the object's state to an opaque memento object (a snapshot) that can be used to restore the state by using the constructor overload.
        public IMemento SaveToMemento() => new Memento
        {
            Version = Version,
            ChargeId = _chargeId,
            Amount = _amount,
            CardInfo = _cardInfo,
            PaymentState = _paymentState
        };

        internal class Memento : IMemento
        {
            public int Version { get; set; }

            public Guid ChargeId { get; set; }

            public PaymentAmount Amount { get; set; }

            public CreditCardInfo CardInfo { get; set; }

            public PaymentState PaymentState { get; set; }
        }
    }
}
