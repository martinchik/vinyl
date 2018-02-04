# Vinyl

#### The source code and descriptions about site <b>http://buy-vinyl.by/ </b> 
<i>(Currently site has only russian language, because works only in Belarus)</i> <br /> <br />
The main goal of this project is - study of new technologies.<br />
I had worked on the project exactly 2 months in free time from work and family.

## How site works

This site is designed to simplify the search and purchase vinyl records first of all in Belarus. <br />
There is no advertising on the site. The site does not sell and distribute records. <br />
The site are only simplify search process linking individual stores, sellers, collectors in a one single store.

### Steps:
- The developer finds a store or store finds him.
- The service analyses store and gets answer how to extract information about the sold records.
- The developer creates a separate "plug-in" for the seller for extracting information from source.
- The plugin is add on the site.
- The extracted information is reduced to the standard view that common to all.
- A price converts automaticly if is not specified in the country's currency. A current exchange rate gets from site www.nbrb.by.
- An information about new musicants and albums is looked and extracted from sites www.discogs.com and www.youtube.com.
- The result is added or updated in the general storage.
- The Information from sellers updates 1-2 times a day.
- If there are no updates about record from seller during 5 days, then this record is removed.

# Technical side

### Technologies:
-	.NET Core 2.0 and ASP.NET PageModel.
-	PostgreSql. 
-	Micro-services architecture.
-	Apache Kafka and Zookeeper.
-	Nginx.
-	Docker and docker compose for deploying. <br />

### Components:
Each component is a separate docker container. All component configuration is described in the docker-compose file.
And each project has a docker file for creating docker image.

-	<b>ParsingJob</b> (Getting data). Every 3 hours service gets a list of parsers for work. All records that could be recognized are sent to a Kafka Topic - "dirty_records".
-	<b>ProcessingJob</b> (Update data in DB). Service works all time. Listens the topic "dirty_records" and checks all entries from it in the database and adds or updates them. For each record, it generates transliterated url names, which contains musicant and album names. New records and records with <b>important</b> changes (for example, price) are saved (or updated) in a separate table "SearchItems" in the database.
 This table contains records in get ready for searching in database and showing on the UI format. Also these entries are sent to topic "find_infos_records".
-	<b>AdditionalInfoJob</b> (Search in third-party sources). Service works all time. Listens the topic "find_infos_records" and searches for all records on www.discogs.com. Search can be improved - currently an algorithm gets a first found release (exactly release) by album, artist and year.
-	<b>AliveValidationJob</b> (Removing old records). Every 6 hours, records that are not updated during 5 last days are deleted.
-	<b>Site for UI</b>. Do search by "SearchItems" table in which full-text search is configured. Also the site generates a sitemap with the list of prepeared url addresses.

## Where 
Site works on one virtual machine with Ubuntu OS. Hosting provider is https://vscale.io/

## Deploying
- VM should have a configured ssh, .net, docker thats all.
- Use remote docker for connecting to docker on VM.
- Run "build command" with docker-compose from local machine and it deploys remotely
<br /> my build command is:<br /> 
<code>docker-compose  -f "docker-compose.yml" -p dockercompose-vinyl up -d --build --force-recreate --remove-orphans</code>

## TODO
- Save all logs in database and keep them 5 days, for example. Send fatal errors via email.
- Save and show application metrics via Graphite and Graphana.
- Authorization through Google, Facebook, VK to register yourself as a private seller
- Add a list of own vinyls for sale. As a private collection.
- Add localizations only for UI - all other parts are localized.
- Add support to work in other countries.
- Add search for currency rates in any country.
- Subscription to "Price was changed", "New records".
