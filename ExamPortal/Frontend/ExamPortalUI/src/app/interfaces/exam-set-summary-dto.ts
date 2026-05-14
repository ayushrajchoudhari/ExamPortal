export interface ExamSetSummaryDto {
  id: string;
  title: string;
  durationMinutes: number;
  passingScore: number;
  isPublic: boolean;
}