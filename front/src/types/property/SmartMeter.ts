export type ConnectionType = 'Monofazni' | 'Trofazni';
export type PairingStatus = 'Unpaired' | 'Paired';

export interface SmartMeter {
    id: string;
    propertyId: string;
    label: string;
    connectionType: ConnectionType;
    maxApprovedPower: number;
    note?: string;
    serialNumber?: string;
    pairingStatus: PairingStatus;
    deviceUUID?: string;
    createdAt: string;
}

export interface AddSmartMeterDto {
    label: string;
    connectionType: ConnectionType;
    note?: string;
}

export interface UpdateSmartMeterDto {
    label: string;
    connectionType: ConnectionType;
    note?: string;
}
