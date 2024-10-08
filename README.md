# Qanat Platform Documentation

Welcome to the Qanat Platform! This guide provides comprehensive instructions for setting up and building the project locally. For any questions, please contact info@groundwateraccounting.org.

---

## Developer Setup Instructions

### Prerequisites

Ensure the following software is installed:

1. **Visual Studio 2022**
2. **Visual Studio Code**
3. **VS Code Extensions:**

   - Angular CLI from Balram Chavan
   - Docker
   - npm support for VS Code from egamma
   - npm commands for VS Code from Florian Knop
   - npm Intellisense from Christian Kohler

4. **Node.js** (Ensure Node.js 18 is used via NVM if needed)
5. **Docker**
6. **Microsoft SQL Server**
7. **.NET 8 SDK**
8. **Git Submodule**  
   This project requires a Git submodule located at: [https://github.com/sitkatech/efcorepocogenerator](https://github.com/sitkatech/efcorepocogenerator). Please follow the readme found there to complete the setup. To properly link the git submodule please run the following command in this repository:

   ```bash
   git submodule update --init --recursive
   ```

### Qanat API Setup

1. **Clone the repository:**  
   Clone the repository to `C:/git/sitkatech` on your development machine.

2. **Set up a SQL user:**  
   Create a SQL user (e.g., `DockerWebUser`) with the `sysadmin` role and the password `password#1`. Ensure that password policies are not enforced.
3. **Configuration files:**  
   Copy the template files and remove .template from their file name to produce the following environment variable files. Reach out if you need help filling these out, as they are secrets and should not be committed to the repository.

   - `.env` (from the `docker-compose` directory)
   - `appsecrets.json` (from the `Qanat.API` directory)
   - `secrets` (from the `Build` directory)
   - Create a file named geoserver-environment.properties with the following contents:
     ```
     datastore-host = host.docker.internal
     datastore-password = password#1
     datastore-user = DockerWebUser
     datastore-database = QanatDB
     ```

4. **Update environment configuration:**  
   Copy `[repo root dir]\docker-compose\.env.template` to `[repo root dir]\docker-compose\.env` and update the values.

5. **Open the solution in Visual Studio 2022:**  
   Set `docker-compose` as the startup project. Press the green play button to start the API server.

---

### Qanat Web Setup

1. **Open the web workspace:**  
   Open the `qanat-web-workspace` in Visual Studio Code (`[repo root dir]\Qanat.Web`).

2. **Install dependencies and start the server:**  
   In the VS Code terminal (or your favorite command line tool), run the following commands:

   - `npm install`
   - `npm build`
   - `npm start`

3. **Debugging:**  
   Press `F5` to launch the web app in Google Chrome and debug JavaScript in Visual Studio Code.

   - You may need to set up a launch configuration to point to `https://qanat.localhost.sitkatech.com`.

4. **Generate models:**  
   Run `npm run gen-model` to generate models. If there’s an issue, use the following commands to resolve it:
   - `npm install @openapitools/openapi-generator-cli -g`
   - `openapi-generator-cli version-manager set 5.3.0`

---

### Local SSL Setup (API and Web)

Local certificates should be generated automatically:

1. **API:**

   - The `Qanat.API.csproj` file includes a post-build step during debugging that executes `dotnet dev-certs https`.

2. **Web:**
   - The `package.json` file has a `prestart` script that checks for local development certificates and attempts to create them if they don't exist.

---

### Tests

The automated tests can be run locally after creating an `environment.json` file within the `Qanat.Tests` project with the correct values. These tests are run on our CI builds and should be fixed as soon as possible to avoid bugs piling up.

---

### Troubleshooting After a Period of Inactivity

If the project doesn't work after not working on it for a while, try the following:

1. **Run `npm install`:**  
   There might be updated or missing packages added by other developers.

2. **Download, Restore and Build the Database:**
   Ensure your database matches the scaffolded entities by rebuilding your database.

3. **Update Environment Variables:**  
   Get the latest `.env`, `appsecrets.json`, `build.ini` files from a colleague as there may be environment variables added since the last time you ran the project.

4. **Reach out to a colleague:**  
   If you’re stuck, ask another developer who has recently worked on the project for help.

---

## Configurations for Instances

### Additional Front End Configuration:

- In the `Qanat.Web\src\environments` folder, update the values in the following TypeScript files to point to your instances's resources:
  - `environment.ts`
  - `environment.qa.ts`
  - `environment.prod.ts`
- In `auth.config.ts`, update the `msalconfig` section to reflect your instance's configuration details.

---

## License

This project is licensed under the **GNU Affero General Public License v3.0 (AGPL-3.0)**.

You may obtain a copy of the License at:

[https://www.gnu.org/licenses/agpl-3.0](https://www.gnu.org/licenses/agpl-3.0)

By using this project, you agree to the terms and conditions of this license, which ensure that any derivative works based on this project must also be open-source and made available to the public under the same terms.

Copyright California Water Data Consortium, Environmental Defense Fund, and Environmental Science Associates. Source code licensed under the AGPL 3.0.
