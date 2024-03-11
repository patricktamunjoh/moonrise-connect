# Changelog

### Update Version 1.1.0
Changed network function invocation syntax from extension methods to static methods
```c#
(12, "example").Send(ExampleNetworkFunction); // Old syntax
Invocation.Invoke(ExampleNetworkFunction, 12, "example"); // New syntax
```