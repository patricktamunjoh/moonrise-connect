# Changelog

### Update Version 1.1.0
Changed network function invocation syntax from extension methods to static methods.
```c#
(12, "example").Send(ExampleNetworkFunction); // Old syntax
Invocation.Call(ExampleNetworkFunction, 12, "example"); // New syntax
```

Renamed `Connect` class to `Session`.

```c#
Connect cloudsAhoyConnect = new Connect.Builder().ForSteam().Build(); // Old name
Session session = new Session.Builder().ForSteam().Build(); // New name
```