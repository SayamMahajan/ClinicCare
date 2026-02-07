import { SpecializationResponse } from "../models/specialization.model";

export function buildSpecializationMap(
  specializations: SpecializationResponse[]
): Map<string, string> {
  return new Map(
    specializations.map(s => [s.id, s.type])
  );
} 