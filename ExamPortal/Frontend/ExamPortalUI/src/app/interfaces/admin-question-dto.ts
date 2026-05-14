import { AdminAnswerOptionDto } from "./admin-answer-option-dto";

export interface AdminQuestionDto {
  id: string | null;
  questionText: string;
  points: number;
  displayOrder: number;
  options: Array<AdminAnswerOptionDto>;
}