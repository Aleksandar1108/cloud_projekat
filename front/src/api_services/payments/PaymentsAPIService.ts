import axios from "axios";
import { buildApiUrl } from "../ApiBase";
import type { Payment } from "../../types/payments/Payment";

export async function getPayments(year: number, month: number): Promise<Payment[]> {
    try {
        const res = await axios.get<Payment[]>(
            buildApiUrl(`api/payments?year=${year}&month=${month}`)
        );
        return res.data;
    } catch (error) {
        if (axios.isAxiosError(error) && error.response?.data) {
            const serverMessage = error.response.data.message ?? error.response.data.error;
            throw new Error(serverMessage ?? "Neuspesno ucitavanje uplata.");
        }
        throw new Error("Neuspesno ucitavanje uplata.");
    }
}

export type CreateCheckoutSessionResponse = {
  sessionId: string;
  url: string;
};

export async function createCheckoutSession(
  deviceId: string,
  year: number,
  month: number
): Promise<CreateCheckoutSessionResponse> {
  try {
    const response = await axios.post<CreateCheckoutSessionResponse>(
      buildApiUrl("api/payments/checkout-session"),
      { deviceId, year, month },
      { headers: { "Content-Type": "application/json" } }
    );

    return response.data;
  } catch (error) {
    if (axios.isAxiosError(error) && error.response?.data) {
      const serverMessage =
        error.response.data.message ??
        error.response.data.error ??
        error.response.data.details;

      throw new Error(serverMessage ?? "Neuspesno kreiranje Stripe sesije.");
    }

    throw new Error("Neuspesno kreiranje Stripe sesije.");
  }
}

