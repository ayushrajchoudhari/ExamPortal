export interface ExamResultDto {
    attemptId: string;
    examTitle: string;
    totalScore: number;
    passingScore: number;
    passed: boolean;
    startTime: string;
    endTime: string;
}
