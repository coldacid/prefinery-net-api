Prefinery .NET API
==================
 
An assembly which makes it easy to integrate Prefinery into your ASP.NET website.
 
Prefinery (http://www.prefinery.com) helps you launch a private beta in minutes.
Prefinery collects email addresses, generates invitation codes, and sends
invitations for your web beta. Your customers never leave your site and
e-mail invitations are sent from your address.
 
 
Installation
============

Build Compulsivio.Prefinery.dll and add it to your website project's references.
It's really that simple.


Configuration
=============

The Prefinery .NET API can be configured via Web.config. See below for an example:

	<configuration>
	  <configSections>
		<section name="prefinery" type="Compulsivio.Prefinery.Configuration.PrefineryConfigHandler, Compulsivio.Prefinery"/>
	  </configSections>
	  <prefinery>
		<account name="sample" apiKey="a6c4aae2f1fc0e147272079ea95b219e26f6beaa" />
		<betas>
		  <beta id="1" name="Sample Beta" decodeKey="46c650bcce7b4e68b4986db07910df63474e4c7f" />
		</betas>
	  </prefinery>
	</configuration>

The <prefinery> section contains an <account> tag for the name of your Prefinery
account, as well as your API key. These are shared between your various betas.
Both the name and apiKey parameters are required.

The <betas> tag contains a collection of <beta> tags, each of which represents one
of your Prefinery betas. The id parameter is the ID number of your beta, the
decodeKey parameter is the key used to checking invite codes, and the name parameter
is so you can identify multiple betas more easily in your website. The id and
decodeKey parameters are required, but name is not.

If you don't want to use Web.config to set up Prefinery, you don't have to... Read
on for details.


Documentation
=============

The PrefineryCore object is the base of the Prefinery .NET API. To do anything with
the API, create a PrefineryCore object. Through PrefineryCore, you can use GetBeta()
to get an object representing the beta you want to manage.

The Beta object allows you to get, add, change and remove testers, each of which can
be individually managed.


License
=======

Copyright (c) 2009 Chris Charabaruk. Released under the MIT license.