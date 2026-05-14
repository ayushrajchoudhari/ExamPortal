export interface CreateExamRequestDto {
  title: string;
  durationMinutes: number;
  passingScore: number;
  instructionContent: string;
  isPublic: boolean;
}