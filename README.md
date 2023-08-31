# Saml2.Samples
Samples on how to use the Sustainsys.Saml2 library. This are designed to be minimal samples used as starting points. The sample
applications that are available together with the source code are used to drive testing and contains all features enabled at once.

## Samples for Sustainsys.Saml2 2.x, found in v2 directory
* AspNetCore: Basic sample on how to wire up Sustainsys.Saml2 with Asp.Net Core, without using Asp.Net Identity.
* AspNetCoreClaimsTransformation: Similar to the previous one, but uses AcsCommandResultCreated notification to transform
  the generated identity before the local session is created.
* DuendeDynamicProviders: Shows how to use Sustainsys.Saml2 with Duende IdentityServer's dynamic provider feature. If you
  just want an easy way to add a fixed Saml2 provider to Duende IdentityServer you should follow the steps in the AspNetCore sample.
