#Diagnostics

- Logger Name
    - Hierarchical
    - Can be used for filtering

- Diagnostic Event
    - Use caller name as logger name
    Cons: 
        reduced performance
    Pros:
        helps to quickly see where log events come from during debugging phase (e.g. when you're starting to work on an existing project and want to know what is happening)   

- Global Properties

- Configuration
    - Settings
        - Use Caller Name As Logger Name
        - Include Caller Info
        - Enable Logging
            Set to False to disable all logging.
            When False, logging methods will return right away, causing least impact on application

    - Global Filter
        Can be used to quickly enable / disable logging on a global level.
            - enable / disable all logging from a specific assembly, namespace, class or method 
            - enable / disable all loging with specific severity

- Sinks
    - Synchronous Sinks
        Blocking, Logging must wait for write, will block thread until all information is written to the sink
    - Asynchronous Sinks
        Non-Blocking, Fire and forget, Logging will send information to the sink without waiting for it to finish processing
- Filters

- Context Data Collectors

    Context Data Collectors provide a way of retrievent information about context in which logging occured.
    They can provide information such as current time, logged-in user name, application version, available memory, other applications running, etc.

    - Requested by sinks (via Formatters)
    Sinks can request context data to be included in log messages by specifying it in formatters

    - Additional
    Additional context data may be appended to only specific events based on various criteria (e.g. Severity Level)

- Type Description

- Fluent Interface

- Audit

