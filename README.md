# SETTUP AND CREATING AN API CLIENT-SERVER HL7-FHIR STU3 v3.0.1 IN WINDOWS Part2

For this second part of the project the characterics of FHIR to be explored will be ADDING,Deleting and UPDATING
a patient in a local server.

# NEEDED TOOLS

For developing this part of the project it will be needed the same software as the previsously project present in the
repository: https://github.com/orlandonss/HL7-FHIR/tree/main/Projeto01.
Also, it will be needed a docker to run a local server and perform some operations.

- [Docker](https://www.docker.com)

# SETUP A LOCAL SERVER IN HL7-FHIR

For running a local server it will be needed to setup and configurate the docker.

First you need to have the docker already installed in your local computer, then it is needed to run certain commands to start the docker and configurate him  to run the fhir application.

# Setup HAPI FHIR Server for STU3 (v3.0.1) Using Docker in PowerShell

## Step 1: Create Required Files and Folders

In PowerShell, run the following commands:

```powershell
New-Item -Path . -Name "docker-compose.yml" -ItemType "File"
New-Item -Path . -Name "config" -ItemType "Directory"
New-Item -Path .\config -Name "hapi.properties" -ItemType "File"
```

---

## Step 2: Edit docker-compose.yml

Open the file in VSCode:

```powershell
code docker-compose.yml
```

Paste this content:

```yaml
version: '3.8'

services:
  hapi-fhir-stu3:
    image: hapiproject/hapi:latest
    container_name: hapi-fhir-stu3
    ports:
      - "8080:8080"
    environment:
      - JAVA_OPTS=-Xms512m -Xmx2048m
    volumes:
      - ./data:/data
      - ./config/hapi.properties:/app/WEB-INF/hapi.properties
```

---

## Step 3: Edit hapi.properties

Open the config file:

```powershell
code .\config\hapi.properties
```

Paste this content:

```properties
hapi.fhir.fhir_version=DSTU3
hapi.fhir.persistence.datasource.url=jdbc:h2:file:/data/hapi-database
hapi.fhir.persistence.datasource.driver=org.h2.Driver
hapi.fhir.server_address=http://localhost:8080/fhir
hapi.fhir.default_response_encoding=json
hapi.fhir.allow_external_references=true
```

---

## Step 4: Run Docker Compose

From your project root (where docker-compose.yml is):

```powershell
docker-compose up -d
```

---

## Step 5: Verify FHIR Server

Open in your browser:

```
http://localhost:8080/fhir/metadata
```

You should see JSON output including:

```json
"fhirVersion": "3.0.1"
```



# OPERATIONS IN HL7-FHIR PATIENT DATA

## ADD PATIENT

## UPDATE PATIENT

## DELETE PATIENT
