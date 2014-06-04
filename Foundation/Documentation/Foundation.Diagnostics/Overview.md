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
















# ROOT ELEMENT
	Root Element name does not matter and can be set to anything
	CLR Type to which Root Element is mapped is determied by type passed to .Deserialize<T>() method (where T is a CLR Type to which Root Element will be mapped to)

	<root_name_does_not_make_a_difference />

# ATTRIBUTES
	Attributes can be used to set values of CLR Properties, where these values can be converted from/to a string
	Attribute Name should be the same as CLR Property Name to which it is mapped

	NOTE: 	Attribute Name => CLD Proeprty Name mapping is *NOT* case sensitive.
			CLR Types which contain properties with the same name (but different character cases, e.g. Root and rOOt) will not serialize properly

	<Root name="some name" />

# ATTACHED ELEMENTS
	Attached Elements can be used to set values of CLR Properties
	Attached Element Name should be the same as CLR Property Name to which it is mapped, preceeded by Parent Element Name followed by a '.'

	<Root>
		<Root.Name>some name</Root.Name>
	</Root>

	NOTE: 	Attribute Name => CLD Proeprty Name mapping is *NOT* case sensitive.
			CLR Types which contain properties with the same name (but different character cases, e.g. Root and rOOt) will not serialize properly

# CHILD ELEMENTS
	When Parent Element is mapped to a CLR Type which is a collection (IList)
	then Child Elements represent individual items in a collection.
	Child Element Name will be used to map element to CLR Type of a child

	<MyCollection>
		<Item />
		<Item />
		<ItemOfDifferentType />
	</MyCollection/>

	When Parent Element is mapped to a CLR Type wich *IS NOT* a collection
	then Child elements represent properties of that CLR Type, that is
	Child Element Name will be used to map element to the CLR Property with the same name


	<Root>
		<Name>some name</Name>
	</Root>

	or using Attached Element:

	<Root>
		<Root.Name>some name</Root.Name>
	</Root>

	NOTE: For collections, use Attached Elements to set collection properties, and Child Elements to set collection items

	<Root id="root">
		<Children>
			<Children.Parent id-ref="root" />

			<Item1 />
			<Item2 />
		</Children>


# RESOLVING CLR TYPE OF A PROPERTY
	Child Element Name is mapped to a CLR Property Name
	For example

	<Root>
		<Children />

	Child Element 'Children' will be mapped to a CLR Property called 'Children' on a CLR Type to which Root element is mapped

	If CLR Property ('Children') is read-only (i.e. does not contain public setter), then it is assumed that CLR Type mapped to parent element ('Root') will create instance of it in its constructor


	public class Root
	{
		public IList<object> Children { get; private set; }

		public Root()
		{
			this.Children = new List<object>();
		}
	}

	If CLR Property ('Children') is *NOT* read-only (i.e. contains public setter),
	AND CLR Property ('Children') Type is a concrete class (i.e. not an abstract class or interface)
	then CLR Property Type will be used

	public class Root
	{
		//! concrete type (not an interface of abstract class)
		public List<object> Children { get; set; }
	}

	<Root>
		<Children>
			<Item />
			<Item />
	

	If CLR Proeprty ('Children') is *NOT* read-only (i.e. contains public setter),
	AND CLR Property ('Children') Type is *NOT* a concrete class (i.e. is an interface of abstract class)
	then collection must be wrapped in a Wrapping Element.
	Wrapping Element Name will be used to map element to the CLR Property with the same name.

	public class Root
	{
		//! interface
		public IList<object> Children { get; set; }
	}

	public class ChildrenCollection : Collection<object>
	{ }

	<Root>
		<Children>
			<ChildrenCollection>
				<Item />
				<Item />

