# poc: tyk-tenant-policies

proof-of-concept evaluating tenant-specific policies using the tyk oss api gateway in conjunction with keycloak issued tokens containing a tenantId claim which is used to determine the policy to apply.

## run the poc

1. run all required services via `docker compose up`
2. execute the http requests in `requests.http` repeatedly 
3. observe `goggle` token having a 0,167rps (10req/60sec) rate limit
4. observe `appel` token having a 1,67rps (100req/60sec) rate limit

## issues

1. currently struggeling to get meaningful _X-Ratelimit-_ headers, when only configuring rate-limiting via _rate_ and _per_. headers only supply valid values when setting *quota_max* to the same value as _rate_ and *quota_renewal_rate* to the same value _per_. otherwise all _X-Ratelimit-_ headers values will be 0

2. seemingly cannot use a combination of the tenant-specific policy (identified by the _tenantId_ token claim) and a default-policy. the latter one is only picked up if no specific one was found. this seems to be by design. possible workaround is to enlist multiple policies in the configured claim. either by array notation (e.g.['appel', 'default']) or by multiple occurences of the relevant claim. did not test either yet..

3. switching to tyk tyk-gateway:v5.9.x breaks the authentication configuration for some reason. while the first request succeeds, each subsequent request leads to a `403 Forbidden` response with the body containing `"error": "Key not authorized"`. in the logs i receive `"Couldn't get token. illegal base64 data at input byte 4" mw=JWTMiddleware`,
`"Attempted JWT access with non-existent key." mw=JWTMiddleware` and 
`"JWT validation error. illegal base64 data at input byte 4"` tyk-gateway v5.9.x would be awesome because we would be able to configure multiple JWKS uris (via `<jwtAuthScheme>.jwksURIs`) instead of having to aggregate all tenant JWKS endpoints into one in the application layer (see https://tyk.io/docs/basic-config-and-security/security/authentication-authorization/json-web-tokens/#remotely-stored-keys-jwks-endpoint)