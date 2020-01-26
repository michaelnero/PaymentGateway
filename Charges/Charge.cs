using System;
using System.Collections.Generic;
using Charges.Contracts.Events;
using Charges.Util;
using Infrastructure.EventSourcing;

namespace Charges
{
    public class Charge : EventSourced, IMementoOriginator
    {
        private DateTimeOffset _createdOn;
        private PaymentAmount _amount;
        private CreditCardInfo _cardInfo;
        private ChargeState _chargeState;
        private PaymentState _paymentState;

        protected Charge(Guid id) : base(id)
        {
            // You'll notice that the public methods don't update any state themselves. Instead, they add events and
            // the handlers update the state. This allows the state of this class to be re-built from events.

            Handles<ChargePlaced>(OnChargePlaced);
            Handles<ChargeUpdated>(OnChargeUpdated);
            Handles<ChargeCancelled>(OnChargeCancelled);
            Handles<ChargeConfirmed>(OnChargeConfirmed);
            Handles<ChargeRejected>(OnChargeRejected);
        }

        public Charge(Guid id, IEnumerable<IVersionedEvent> history) : this(id)
        {
            LoadFrom(history);
        }

        public Charge(Guid id, IMemento memento, IEnumerable<IVersionedEvent> history) : this(id)
        {
            var state = (Memento)memento;

            Version = state.Version;

            _createdOn = state.CreatedOn;
            _amount = state.Amount;
            _cardInfo = state.CardInfo;
            _chargeState = state.ChargeState;
            _paymentState = state.PaymentState;

            LoadFrom(history);
        }

        public Charge(Guid id, PaymentAmount amount, CreditCardInfo cardInfo) : this(id)
        {
            if (amount is null) throw new ArgumentNullException(nameof(amount));
            if (cardInfo is null) throw new ArgumentNullException(nameof(cardInfo));

            Update(new ChargePlaced
            {
                CreatedOn = DateTimeOffset.UtcNow,
                Amount = amount.Value,
                Currency = amount.Currency,
                CardNumber = cardInfo.CardNumber,
                Cvv = cardInfo.Cvv,
                ExpiryMonth = cardInfo.ExpiryMonth,
                ExpiryYear = cardInfo.ExpiryYear
            });
        }

        public DateTimeOffset CreatedOn => _createdOn;

        public PaymentAmount Amount => (PaymentAmount)_amount.Clone();

        public CreditCardInfo CardInfo
        {
            get
            {
                var cloned = (CreditCardInfo)_cardInfo.Clone();
                cloned.CardNumber = CardNumberUtil.Mask(cloned.CardNumber);

                return cloned;
            }
        }

        public ChargeState ChargeState => _chargeState;

        public PaymentState PaymentState => _paymentState;

        public void UpdateAmount(PaymentAmount amount)
        {
            Update(new ChargeUpdated
            {
                Amount = amount.Value,
                Currency = amount.Currency
            });
        }

        public void Cancel()
        {
            Update(new ChargeCancelled());
        }

        public void Confirm()
        {
            Update(new ChargeConfirmed());
        }

        public void Reject()
        {
            Update(new ChargeRejected());
        }

        private void OnChargePlaced(ChargePlaced e)
        {
            _createdOn = e.CreatedOn;
            _chargeState = ChargeState.Pending;
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

        private void OnChargeUpdated(ChargeUpdated e)
        {
            _amount = new PaymentAmount
            {
                Value = e.Amount,
                Currency = e.Currency
            };
        }

        private void OnChargeCancelled(ChargeCancelled e)
        {
            _chargeState = ChargeState.Completed;
        }

        private void OnChargeConfirmed(ChargeConfirmed e)
        {
            _chargeState = ChargeState.Completed;
            _paymentState = PaymentState.Success;
        }

        private void OnChargeRejected(ChargeRejected e)
        {
            _chargeState = ChargeState.Completed;
            _paymentState = PaymentState.Failed;
        }

        // Saves the object's state to an opaque memento object (a snapshot) that can be used to restore the state by using the constructor overload.
        public IMemento SaveToMemento() => new Memento
        {
            Version = Version,
            Amount = _amount,
            CardInfo = _cardInfo,
            ChargeState = _chargeState,
            PaymentState = _paymentState
        };

        internal class Memento : IMemento
        {
            public int Version { get; set; }

            public DateTimeOffset CreatedOn { get; set; }
            
            public PaymentAmount Amount { get; set; }

            public CreditCardInfo CardInfo { get; set; }

            public ChargeState ChargeState { get; set; }

            public PaymentState PaymentState { get; set; }
        }
    }
}
