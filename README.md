# JointCode.Shuttle
JointCode.Shuttle is a fast, flexible and easy-to-use service framework for inter-AppDomain communication. It's a replacement for MarshalByrefObject which is provided by the runtime libraries.

1. Service (interface) oriented.
2. Access any AppDomain service from an AppDomain.
3. Better performance: 60 ~ 70 faster than MarshalByrefObject.
4. Service is manageable: you can dynamically register / unregister service group.
5. Strong type, easy to use (while the MarshalByrefObject way relies on magic string to find the service type).
6. Built-in IoC functionality for automatic service dependencies management.
7. Supports for lazy type / assembly loading.
8. The remote service lifetime can be managed by leasing, or by user (while the MarshalByrefObject way does not provide remote service       life management).
9. Simple and quick to get started.
10. Support .net 2.0.
