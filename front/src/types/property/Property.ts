export type PropertyType = 'Stan' | 'Kuca' | 'Vikendica';

export interface Property {
    id: string;
    userId: string;
    name: string;
    city: string;
    address: string;
    description?: string;
    propertyType: PropertyType;
    createdAt: string;
}

export interface CreatePropertyDto {
    name: string;
    city: string;
    address: string;
    description?: string;
    propertyType: PropertyType;
}

export interface UpdatePropertyDto {
    name: string;
    city: string;
    address: string;
    description?: string;
    propertyType: PropertyType;
}
