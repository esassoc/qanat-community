const fs = require("fs");

const basePath = process.argv[2];
const modelsPath = basePath + "/model";
const apiPath = basePath + "/api";

function separateModelsAndApis(filePath) {
    return new Promise((resolve, reject) => {
        fs.readFile(filePath, "utf8", (err, data) => {
            if (err) {
                reject(err);
                return;
            }

            const lines = data.split("\n"); // Split by newlines
            const models = [];
            const apis = [];

            for (const line of lines) {
                if (line.startsWith("model/")) {
                    const filePath = `${modelsPath}/${line.split("/")[1].replace("\r", "")}`;
                    models.push(filePath);
                } else if (line.startsWith("api/")) {
                    const filePath = `${apiPath}/${line.split("/")[1].replace("\r", "")}`;
                    apis.push(filePath);
                }
            }

            resolve({ models, apis });
        });
    });
}

// Usage example
separateModelsAndApis(basePath + "/.openapi-generator/files") // Replace with your actual file path
    .then((data) => {
        cleanPathWithExistingFiles(modelsPath, data.models).then((modelsToDelete) => {
            console.log("Deleting Model files:", modelsToDelete);
            modelsToDelete.forEach((model) => fs.unlinkSync(model));
        });
        cleanPathWithExistingFiles(apiPath, data.apis).then((apiServicesToDelete) => {
            console.log("Deleting API Services:", apiServicesToDelete);
            apiServicesToDelete.forEach((model) => fs.unlinkSync(model));
        });
    })
    .catch((err) => {
        console.error("Error reading file:", err);
    });

function cleanPathWithExistingFiles(modelsPath, existingModels) {
    return new Promise((resolve, reject) => {
        fs.readdir(modelsPath, (err, files) => {
            if (err) {
                reject(err);
                return;
            }
            const models = files.filter((file) => file.endsWith(".ts")).map((file) => `${modelsPath}/${file}`);

            const modelsToDelete = models.filter((model) => !existingModels.includes(model));

            resolve(modelsToDelete);
        });
    });
}
