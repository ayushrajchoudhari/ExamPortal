import { ActiveAnswerOptionDto } from "./active-answer-option-dto";

export interface ActiveQuestionDto {
    id: string; 
      questionText: string; 
      points: number; 
      options: Array<ActiveAnswerOptionDto>;
}
