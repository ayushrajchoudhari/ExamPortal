import { AdminQuestionDto } from "./admin-question-dto";

export interface AdminExamDetailsDto {
  id: string;
  title: string;
  durationMinutes: number;
  passingScore: number;
  isPublic: boolean;
  secretToken: string | null;
  instructionContent: string;
  questions: Array<AdminQuestionDto>;
}