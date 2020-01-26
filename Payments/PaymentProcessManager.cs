using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Infrastructure.Messaging;
using Infrastructure.Processes;
using Payments.Contracts.Commands;
using Payments.Contracts.Events;

namespace Payments
{
    public class PaymentProcessManager : IProcessManager
    {
        private readonly List<Envelope<ICommand>> _commands;

        public enum ProcessState
        {
            NotStarted = 0,
            SentToAcquiringBank = 1,
            ReceivedFromAcquiringBank = 2,
        }

        public PaymentProcessManager()
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

        public void Handle(PaymentInitiated e)
        {
            if (State == ProcessState.NotStarted)
            {
                ChargeId = e.ChargeId;
                State = ProcessState.SentToAcquiringBank;

                AddCommand(new SendToAcquiringBank
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

        public void Handle(FundsWithdrawSucceeded e)
        {
            if (State == ProcessState.SentToAcquiringBank)
            {
                State = ProcessState.ReceivedFromAcquiringBank;
                Completed = true;

                AddCommand(new ConfirmPayment
                {
                    ChargeId = ChargeId
                });
            }
        }

        public void Handle(FundsWithdrawFailed e)
        {
            if (State == ProcessState.SentToAcquiringBank)
            {
                State = ProcessState.ReceivedFromAcquiringBank;
                Completed = true;

                AddCommand(new RejectPayment
                {
                    ChargeId = ChargeId
                });
            }
        }

        private void AddCommand<T>(T command) where T : ICommand
        {
            _commands.Add(Envelope.Create<ICommand>(command));
        }
    }
}
