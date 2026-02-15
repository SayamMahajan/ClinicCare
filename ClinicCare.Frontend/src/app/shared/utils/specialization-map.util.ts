import { SpecializationResponseDto } from "../models/specialization.model";
export function buildSpecializationMap(
  specializations: SpecializationResponseDto[]
): Map<string, string> {
  return new Map(
    specializations.map(s => [s.id, s.type])
  );
} 