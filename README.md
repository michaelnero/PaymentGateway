
# Payment Gateway
Thanks for taking the time to review my code submission! I've outline below what I've implemented and then some notes and specific improvements I'd like to see if this were a full payment gateway.

## What I've implemented

### PaymentGateway
This is the Asp.Net Core Web API project where the implementation lives. It's structure is intended to be simple and familiar, but here's a road map to get you oriented.

- **/Authentication** - contains classes that supports authenticating clients with an API key.
- **/Contracts** - Input/Output models used to communicate with our controllers
- **/Controllers** - Our fancy controllers
- **/Model** - The DbContext and some classes that are used to support submitted charges
- **/Services** - Right now just houses the acquiring bank service
- **/Util** - Contains classes that support credit card masking and helpers for detecting unique index violations

#### ChargesController
This is the only implemented controller in this project. You'll see that this controller is decorated with the `[Authorize]` attribute, meaning that you need an API key in your header to call any action method here.

I struggled with how "production-like" to make this controller. Ultimately, I settled for making the code as simple as possible so that reviewers wouldn't have to jump around to understand my code. I wouldn't typically have this much logic for more complicated scenarios, instead having minimal code in the action method and relying on something like the mediator pattern.

##### [GET api/charges](https://github.com/michaelnero/PaymentGateway/blob/7c13466e0d208ba7b9545245d081adb80d272d59/PaymentGateway/Controllers/ChargesController.cs#L34)
This action method is pretty simple: it looks up a charge in the DB and returns it if it's found or a 404 if it's not.

##### [POST api/charges](https://github.com/michaelnero/PaymentGateway/blob/7c13466e0d208ba7b9545245d081adb80d272d59/PaymentGateway/Controllers/ChargesController.cs#L51)
I would make this method more resilient to failure in a more production-like scenario. The alternate flow would look like this:
