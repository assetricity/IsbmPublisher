# ISBM Publisher

A .NET application that provides a GUI to publish the contents of an XML file to an OpenO&M ws-ISBM compliant 1.0 server.

ws-ISBM server endpoints and default configuration options are set in the `app.config` / `IsbmPublisher.exe.config` files.

The default `app.config` expects a HTTPS endpoint. Ensure that appropriate certificates have been installed on your machine using the Microsoft Management Console Certificates snap-in. If you want to use a non-secure HTTP endpoint, switch to using a `basicHttpBinding` instead of a `customBinding`.

## License

Copyright 2014 [Assetricity, LLC](http://assetricity.com)

ISBM Publisher is released under the MIT License. See [LICENSE](https://github.com/assetricity/IsbmPublisher/blob/master/LICENSE) for details.
