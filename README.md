Please use this steps to configure application for start

1. Install SQL Server on machine

	Run migration for creating database on local machine

	In ApplicationIdentityDbContext.cs in connection string change server name to your sql server instance, 
	also do this in appsettings.json file in Homework.Web.Api project

	In package manager console Default project must be Homework.Persistance

	Homework.Web.Api must be set as "Set as startup project"

	Then also in package manager console run the command EntityFrameworkCore\update-database -context ApplicationIdentityDbContext which will create database in sql management studio

2. Download devexpress 24.1 version from link https://1drv.ms/u/s!Ao8YxgElW3j2gfVOMBEjDvWwNT_MTA?e=v8hIvX and then install

3. Right click on solution, then properties and in Multiple startup projects set Start in Homework.Web.Api and Homework.AspNetCoreMvc

4. If you want to consume Homework.Web.Api as container with docker you have to install docker engine on your machine (Docker Desktop)  
