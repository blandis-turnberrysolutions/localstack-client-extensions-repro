# localstack-client-extensions-repro
Reproduce issue with LocalStack.Client.Extensions

On the main branch, if you set useLocalStack to false in the appsettings.json it will error with:

```LocalStackClientConfigurationException: ClientFactory<T> missing constructor with AWSOptions parameter.```

if you set useLocalStack to true everything works fine.

On the workaround branch, both useLocalStack true and false will work.