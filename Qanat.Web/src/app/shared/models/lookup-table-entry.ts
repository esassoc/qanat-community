export class LookupTableEntry {
    Name: string;
    DisplayName: string;
    Value?: number;
    constructor(name: string, displayName: string, value: number) {
        this.Name = name;
        this.DisplayName = displayName;
        this.Value = value;
    }
}
