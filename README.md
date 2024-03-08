# Example C# Azure Function instrumented by New Relic
This project provides an example Azure Function instrumented by New Relic taking messages from a Service Bus.

## How it works
Takes messages from the Servce Bus which have had the trace headers added as application properties (see this [repo](https://github.com/beanie999/mikeBHttpDNFEExample)).

## Setup
- Ensure Application Insights is switched off.
- Add NewRelic.Agent.Api and NewRelic.Agent to the project.

Add the following configuration settings:
- NEW_RELIC_LICENSE_KEY and NEW_RELIC_APP_NAME.
- CORECLR_ENABLE_PROFILING and CORECLR_PROFILER (see the [documentation](https://docs.newrelic.com/docs/apm/agents/net-agent/install-guides/install-net-agent-using-nuget/)).
- CORECLR_PROFILER_PATH (for example /home/site/wwwroot/newrelic/libNewRelicProfiler.so on linux).
- CORECLR_NEWRELIC_HOME (for example /home/site/wwwroot/newrelic on linux).

Add code for annotation and custom attrubutes:
- Annotate the main method as a non web transaction `[NewRelic.Api.Agent.Transaction(Web = false)]`.
- Accept the trace headers via the Getter.
- Add custom attributes to the transaction in New Relic `transaction.AddCustomAttribute("queueName", queueName)`. 
