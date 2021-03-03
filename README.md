#Purpose#

This is the PCL port of official .NET SDK for the transloadit.com API.

You can use it to setup file upload forms for your users, or to upload existing files from your server.

#Nuget#

Available in NuGet https://www.nuget.org/packages/TransloaditPCL/

#Directory structure#

- ***"build"*** - added to .gitignore (directory will be created automatically when the class libraries are built on your workspace)
- ***"src"*** - whole source of the SDK
- ***"test"*** - source of unit tests

#Examples#

Transloadit .NET SDK is easy to configure, easy to use, easy to develop and easy to extend. Please see the details below.

##Sample application##

The sample application below is the part of a simple console application. You can use that example in each kind of application (desktop applications, web applications, mobile applications).

###1. Upload file, use pre-created template with all required settings###

```C#
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

using Transloadit;
using Transloadit.Assembly;
using Transloadit.Log;

namespace TestConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			//Create Transloadit instance with your API credentials
			ITransloadit transloadit = new Transloadit.Transloadit("YOUR-PUBLIC-API-KEY", "YOUR-SECRET-KEY");
			
			//Set template ID
			transloadit.SetTemplateID("YOUR-PRECREATED-TEMPLATE-ID");
			
			//Create assembly builder to build up the assembly
			IAssemblyBuilder assembly = new AssemblyBuilder();

			//Add a file to be uploaded
			assembly.AddFile("file-key", FILE-BYTE[]-HERE);
           
			//Invoke assembly, and wait for the result
			TransloaditResponse response = transloadit.InvokeAssembly(assembly);
			
			if (response.Success)
			{
				LoggerFactory.GetLogger().LogInfo(Type.GetType("TestConsole.Program"), "Assembly {0} result", response.Data["assembly_id"]);
			}
			else
			{
				LoggerFactory.GetLogger().LogError(Type.GetType("TestConsole.Program"), "Error has occured while completing assembly");
			}
			Console.ReadLine();
		}
	}
}
```

###2. Upload file, with custom assembly with all required also with optional settings###

```C#
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

using Transloadit;
using Transloadit.Assembly;
using Transloadit.Log;

namespace TestConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			//Create Transloadit instance with your API credentials
			ITransloadit transloadit = new Transloadit.Transloadit("YOUR-PUBLIC-API-KEY", "YOUR-SECRET-KEY");
				
			//Create assembly builder to build up the assembly
			IAssemblyBuilder assembly = new AssemblyBuilder();

			//Add a file to be uploaded
			assembly.AddFile("file-key", FILE-BYTE[]-HERE);
			
			//Define the step, you can define more in the same assembly
			IStep step = new Step();
			step.SetOption("robot", "/image/resize");
			step.SetOption("width", 75);
			step.SetOption("height", 75);
			step.SetOption("resize_strategy", "pad");
			step.SetOption("background", "#000000");
			
			//Add the step to the assembly
			assembly.AddStep("thumb", step);
			
			//Set notification URL
			assembly.SetNotifyURL("http://your-service.net/ready"); 
			
			//Set the expiration date time of the request for the assembly
			//Assembly will be expired in 120 minutes from now
			assembly.SetAuthExpire(DateTime.Now.AddMinutes(120));
			
			//Invoke assembly, and wait for the result
			TransloaditResponse response = transloadit.InvokeAssembly(assembly);
			
			if (response.Success)
			{
				LoggerFactory.GetLogger().LogInfo(Type.GetType("TestConsole.Program"), "Assembly {0} result", response.Data["assembly_id"]);
			}
			else
			{
				LoggerFactory.GetLogger().LogError(Type.GetType("TestConsole.Program"), "Error has occured while completing assembly");
			}
            Console.ReadLine();
		}
	}
}
```

#Transloadit .NET API#

You need to download the necessary assembly and integrate it to your application. Also you can pull the source and compile with your application.

##1. Configuration##

You can configure this SDK via some constant values.

###1.1. Logger configuration

- `Transloadit.Log.LoggerFactory.LoggerClass` - defines the class to be used for logging
- `Transloadit.Log.TransloaditLogger.Enabled` - enables logging with default TransloaditLogger 

##2. Use of the SDK##

If you would like to use .NET SDK for Transloadit in your application, you need to add it as a reference, or you need to pull the latest commit, and use the source directly.

Transloadit services work with assemblies. An assembly must contain each information which will be used for authentication and processing. Each assembly must contain authentication information and step(s) or template ID. You can set custom, and optional values like custom fields and files too.

> **Note:** template ID is the ID of a Transloadit templates that can be defined on Transloadit admin surface

###2.1. Use of `Transloadit.Transloadit` class###

By that class you are able to create a Transloadit instance, which will process all the assemblies sent by your application.

`ITransloadit transloadit = new Transloadit("YOUR-PUBLIC-API-KEY", "YOUR-SECRET-KEY");`

> **Note:** You can use Transloadit instance as singleton instance.

###2.2. Build an assembly###

To build up an assembly you need to use `Transloadit.Assembly.AssemblyBuilder`.

`IAssemblyBuilder builder = new AssemblyBuilder();`

As described above, you can define steps for an assembly, you can use `Transloadit.Assembly.Step` class for that. Each step must have option(s), can be set by `SetOption(string key, object value)` method. That step will be proceed on your uploaded, or predefined resources.

```C#
//Step below will resize the uploaded image to 75x75 size
//with pad resize strategy also with #000000 background color

IStep step = new Step();
step.SetOption("robot", "/image/resize");
step.SetOption("width", 75);
step.SetOption("height", 75);
step.SetOption("resize_strategy", "pad");
step.SetOption("background", "#000000");
```

To add that step to your assembly you need to call the `AddStep(string name, IStep step)` method, where the `name` parameter is the key of the step. You can refer to that key in further steps in the same assmebly, even if you add more.

`builder.AddStep("resize", step);`

####2.2.1. Add custom field to the assembly####

Custom fields can be set for each assembly as a parameter of a precreated template. You can set custom fields with `SetField(string key, object value)` method, where the `key` parameter is the unique key of the field and `value` parameter is the value of the field.

If a valid key is not defined in the custom field collection, then it will be created, also the related value will be set. If a valid key is already defined then it will be overriden with the passed value.

There are some field keys which are used by the SDK. You are not able to use these keys as custom field keys such as "notify_url", "params", "signature", "template_id". If you try to use one of those keys, then a `Transloadit.Assembly.Exceptions.InvalidKeyFieldKeyException` will be thrown.

`Transloadit.Assembly.Exceptions.AlreadyDefinedFieldKeyException` will be thrown, if you try to use a custom field key, which is already defined as a key of a file (read about files in the next section below).

`builder.SetField("field_key", "field_value");`

####2.2.2. Add file to the assembly####

You can call `AddFile(string key, byte[] file)` method - where the `key` parameter is the unique key of the file and the `file` parameter is the content (byte[]) of of the file - on the created `builder` object to add a file to your assembly. 


####2.2.3. Set authentication information####

As described above, you can set manually authentication information. Some methods give you the possibility to do that, please see examples below. Methods can be called on the `builder` object.

- `SetAuthExpires(DateTime date)` - sets the request expiration date
- `SetAuthMaxSize(int size)` - sets the max size (in bytes) of the requests 

```C#
builder.SetAuthExpires(DateTime.Now.AddMinutes(120)); //Request will be expired after the current date time + 120 minutes
builder.SetAuthMaxSize(1024);
```

> **Note:** These methods are optional

####2.2.4. Set notify URL####

You can define notify URL for your assembly by calling `SetNotifyURL(string notifyURL)` on the `builder` object, which will be requested after the assembly is invoked on Transloadit server.

`builder.SetNotifyURL("http://your-service.net/ready");`

If your assembly is ready, then a POST request will be sent to the specified notify URL. By that request you will get information about the status of the created assembly by the posted `transloadit` field.
If you get `assembly_url` as a GET parameter, then that URL will be called by a TransloaditRequest.

> **For example:** You have a mobile .NET application, which invokes assemblies via that .NET SDK, and you have a REST API service written in PHP, which can handle the request which will be sent after the assembly is created. What you need to do is to integrate PHP Transloadit SDK to your PHP REST API service and call `Transloadit::response()` method, then you will be able to use the result of the assembly in your service (like for creating database records, or do something).

####2.2.5. Set template ID####

You can define the template ID which will be used to proceed your assembly on Transloadit server. You can use `SetTemplateID(string templateID)` for that, where the `templateID` parameter is the ID of the precreated template.

`builder.SetTemplateID("ID-OF-PRECEREATED-TEMPLATE");`

####2.2.6. Create assembly on Transloadit####

After your assembly is built up you can send it to Transloadit server by `InvokeAssembly(IAssmeblyBuilder builder)` method - where the `builder` parameter is the built up instance of `AssemblyBuilder` class - that can be called on `transloadit` object. The result of the assembly will be represented in an `ITransloaditJsonResponse` implementation.

`ITransloaditJsonResponse response = transloadit.InvokeAssembly(builder);`

After the request is done and `response` object was created, you are able to use it to handle the result by some `response` object properties, please see them below.

- `response.Data` - gets the response as an object (JObject, provided by the third party JSON handler), which stores the associative tree
- `response.ResponseString` - gets the response string
- `response.Success` - gets the success of the request

> **Note:** You can refer to parsed JSON values by string keys in `response.Data`
> **Further Notes:** If you need the full JSON for a completed assembly then you will need to long poll the Assembly at URLs of the following format, `https://api2.transloadit.com/assemblies/{assembly_id}`. You can read more on Transloadit's suggested polling methodology [here](https://transloadit.com/docs/api/#assemblies-assembly-id-get). Below is an example construction of the polling url given a TransloaditResponse object instance.

```c#
var response = transloadIt.InvokeAssembly(assembly);
string polling_url = $"https://api2.transloadit.com/assemblies/{response.Data["assembly_id"].ToString()}";
```

####2.2.7. Delete assembly on Transloadit####

Transloadit .NET SDK gives you the possibility to delete an assembly on the server. You can call `DeleteAssembly(string assemblyID)` method - where `assemblyID` parameter is the ID of an exisiting assembly - on the created `transloadit` object. That call can be useful, when you would like to cancel your assembly invoke process.

`ITransloaditJsonResponse response = transloadit.DeleteAssembly("YOUR-CREATED-ASSEMBLY-ID");`

You can handle the response like in case of invoke an assembly.

###2.3. Use of Transloadit logger###

As described above the SDK provides you the possibility to log information and errors. You can use the default `Transloadit.Log.TransloaditLogger`, also you can implement the `Transloadit.Log.ITransloaditLogger` interface to create a custom one.

Logger object can be accessed by the `Transloadit.Log.LoggerFactory.GetLogger()` method. That static factory will get the singleton `Transloadit.Log.ITransloaditLogger` implementation. The returned type can be configured (see the description above). One method provides the possibility to log info, also some other methods provides the possibility to log errors, please see the examples below.

- `LogInfo(Type type, string message, params object[] parameters)` - logs information, where the `type` is the type of the sender object, `message` is the custom log message with parameters and `parameters` are the parameters of the custom log message 
- `LogError(Type type, Exception exception, string message, params object[] parameters)` - logs error (preferred to use in case of an exception thrown), where the `type` is the type of the sender object, `exception` is the thrown exception, `message` is the custom log message with parameters and `parameters` are the parameters of the custom log message

> **Note: ** `LogError(...)` method has more definition

##3. Extend Transloadit .NET API##

The SDK is able to be extended. You can use the interfaces to create own implementation, also you can extend the precreated classes to have an extended SDK. Transloadit logger is preferred to be extended.

###3.1. Extend Transloadit logger###

You need to create a class, which implements `Transloadit.Log.ITransloaditLogger` interface. After you have a working implementation, you can set path of your class in the `Transloadit.Log.LoggerFactory.LoggerClass` constant.

`Transloadit.Log.LoggerFactory.LoggerClass = "YourLogger.LoggerForTransloadit, YourLogger";`

The `Transloadit.Log.LoggerFactory.LoggerClass` constant represents the whole definition of your custom logger, where `YourLogger.LoggerForTransloadit` is the exact definition of the logger class and comma separated `YourLogger` is the name of the assembly, which stores your custom logger. `YourLogger.LoggerForTransloadit` must implement `Transloadit.Log.ITransloaditLogger` interface.

##4. Run unit tests##

To pass all the unit tests you need NUnit to be installed on your machine. Update (replace **"YOUR-PUBLIC-API-KEY"** and **"YOUR-SECRET-KEY"** to your keys) your API credentials in the test cases, then compile the test project, then use "Tests.dll" for testing.

> **Note: ** You need to compile the **"Tests"** project.
