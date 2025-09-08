export class FileResponseHelper {
    public static downloadBlobResponse(response: Blob, fileName: string) {
        const blob = new Blob([response], { type: response.type });

        const url = window.URL.createObjectURL(blob);
        const save = document.createElement("a");
        if (typeof save.download !== "undefined") {
            // If the download attribute is supported (most browsers except IE), save.download will return empty string, if not supported, it will return undefined
            // More info on support: https://caniuse.com/download
            save.href = url;
            save.target = "_blank";

            // regex out bad chars and turn spaces into dashes
            const badCharRegex = /[^aA-zZ 0-9-]/g;
            const spaceRegex = /[ ]/g;
            fileName = fileName.replace(badCharRegex, "");
            fileName = fileName.replace(spaceRegex, "-");

            save.download = fileName;
            save.dispatchEvent(new MouseEvent("click"));
        } else {
            // If download attribute is not supported, open the url in a new tab for backwards compatibility
            window.open(url);
        }
    }
}
