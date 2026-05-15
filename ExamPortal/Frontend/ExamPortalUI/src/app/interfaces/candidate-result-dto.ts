export interface CandidateResultDto {
  email: string;
  startTime: string;
  endTime: string | null;
  totalScore: number | null;
  passed: boolean;
}