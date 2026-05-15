import { CandidateResultDto } from "./candidate-result-dto";

export interface AdminExamReportDto {
  examTitle: string;
  passingScore: number;
  candidates: Array<CandidateResultDto>;
}