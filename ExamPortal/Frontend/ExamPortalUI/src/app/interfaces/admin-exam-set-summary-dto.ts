export interface AdminExamSetSummaryDto {
  id: string;
  title: string;
  durationMinutes: number;
  passingScore: number;
  isPublic: boolean;
  questionCount: number;
  secretToken: string | null;
}