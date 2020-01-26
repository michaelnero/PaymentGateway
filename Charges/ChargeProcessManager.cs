using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Charges.Contracts.Commands;
using Charges.Contracts.Events;
using Infrastructure.Messaging;
using Infrastructure.Processes;
using Payments.Contracts.Commands;
using Payments.Contracts.Events;

namespace Charges
{
    public class ChargeProcessManager : IProcessManager
    {
        private readonly List<Envelope<ICommand>> _commands;

        public enum ProcessState
        {
            NotStarted = 0,
            AwaitingPaymentConfirmation = 1,
            PaymentConfirmationReceived = 2,
        }

        public ChargeProcessManager()
        {
            _commands = new List<Envelope<ICommand>>();
            Id = Guid.NewGuid();
        }

        public IEnumerable<Envelope<ICommand>> Commands => _commands;

        public Guid Id { get; private set; }
        
        public Guid ChargeId { get; private set; }
        
        public bool Completed { get; private set; }
        
        public ProcessState State { get; private set; }

        [ConcurrencyCheck, Timestamp]
        public byte[] RowKey { get; set; }

        public void Handle(ChargePlaced e)
        {
            if (State == ProcessState.NotStarted)
            {
                ChargeId = e.SourceId;
                State = ProcessState.AwaitingPaymentConfirmation;

                AddCommand(new InitiatePayment
                {
                    ChargeId = ChargeId,
                    Amount = e.Amount,
                    Currency = e.Currency,
                    CardNumber = e.CardNumber,
                    Cvv = e.Cvv,
                    ExpiryMonth = e.ExpiryMonth,
                    ExpiryYear = e.ExpiryYear
                });
            }
        }

        public void Handle(ChargeUpdated e)
        {
            if (State != ProcessState.AwaitingPaymentConfirmation)
            {
                throw new InvalidOperationException("Cannot update the charge at this time.");
            }

            State = ProcessState.AwaitingPaymentConfirmation;
            
            AddCommand(new UpdatePayment
            {
                ChargeId = ChargeId,
                Amount = e.Amount,
                Currency = e.Currency
            });
        }

        public void Handle(ChargeCancelled _)
        {
            Completed = true;

            AddCommand(new CancelPayment
            {
                ChargeId = ChargeId
            });
        }

        public void Handle(PaymentCompleted _)
        {
            if (State != ProcessState.AwaitingPaymentConfirmation)
            {
                throw new InvalidOperationException("Cannot handle payment confirmation at this stage.");
            }

            Completed = true;
            State = ProcessState.PaymentConfirmationReceived;

            AddCommand(new ConfirmCharge
            {
                ChargeId = ChargeId
            });
        }

        public void Handle(PaymentRejected _)
        {
            if (State != ProcessState.AwaitingPaymentConfirmation)
            {
                throw new InvalidOperationException("Cannot handle payment confirmation at this stage.");
            }

            Completed = true;
            State = ProcessState.PaymentConfirmationReceived;

            AddCommand(new RejectCharge
            {
                ChargeId = ChargeId
            });
        }

        private void AddCommand<T>(T command) where T : ICommand
        {
            _commands.Add(Envelope.Create<ICommand>(command));
        }
    }
}
