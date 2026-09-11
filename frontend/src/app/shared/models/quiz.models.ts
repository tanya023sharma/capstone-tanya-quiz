export type Difficulty = 'easy' | 'medium' | 'hard';
export type OptionLabel = 'A' | 'B' | 'C' | 'D';

export interface GenerateQuizRequest {
  topicContext?: string;
  sourceText: string;
  difficulty: Difficulty;
}

export interface QuizResponse {
  id: string;
  topicContext?: string;
  difficulty: Difficulty;
  questions: PublicQuestion[];
  expiresAt: string;
}

export interface PublicQuestion {
  id: string;
  number: number;
  prompt: string;
  options: PublicOption[];
}

export interface PublicOption {
  label: OptionLabel;
  text: string;
}

export interface SubmittedAnswer {
  questionId: string;
  selectedOption: OptionLabel;
}

export interface QuizResults {
  quizId: string;
  score: number;
  totalQuestions: number;
  results: QuestionResult[];
}

export interface QuestionResult {
  questionId: string;
  number: number;
  prompt: string;
  selectedOption: OptionLabel;
  correctOption: OptionLabel;
  isCorrect: boolean;
  options: ResultOption[];
  evidence: EvidenceExcerpt[];
}

export interface ResultOption {
  label: OptionLabel;
  text: string;
  isCorrect: boolean;
  explanation: string;
}

export interface EvidenceExcerpt {
  quote: string;
  sourceStart: number;
  sourceEnd: number;
}
