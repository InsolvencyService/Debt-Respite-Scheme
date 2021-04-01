# Insolvency

## Components

### Identity

The Authentication and Authorization component is composed of a third party Identity Server offering, a custom Identity Management, custom on boarding, and integration into the government SCP (Secure Credential Platform).

The Authentication task is offloaded to the SCP, which will issues a valid token that will link to an on boarded organisation. The on boarding is carried out via API exposed and the matching is carried out using an initial email address.

Once an organisation has successfully on boarded and authorized by way of a valid token with a matching email, a new token is issued for the authorization part with a valid list of claims.

The Identity Management part if accessible to users who have been set-up as an admin, which allows that user, manage further users and set-up client credential details for API access.

### Portal

The Portal is a .Net core App Service that provide a UI to interact with an API Integration back-end.

### Integration

The Integration layer is .Net core App Service that provides APIs both to provide the functionality to the Portal front-end and the API’s for the external parties (Creditors and Money Advisors)

### Notifications

The Notifications component is based on a service bus (Azure Service Bus) event based architecture, which integrates with the government Notify platform. Messages are sent onto the service bus with the relevant meta data for each message type required, letter, email or api. This meta data is used to route the messages to the relevant Azure Functions (Paas).

For letter and email notifications, the functions send the request to notify and if successful, these are written to a database. The database codebase is generated using Entity Framework from the domain model.

## Build instructions

Each different component can be build by opening the solution in Visual Studio and rebuilding. An alternative would be to navigate to the root solution folder and run `dotnet build`.

### Running locally

This project requires a properly set up environment including SCP, Dynamics and Azure infrastructure. Without access to such an environment, running the components locally is not possible.
