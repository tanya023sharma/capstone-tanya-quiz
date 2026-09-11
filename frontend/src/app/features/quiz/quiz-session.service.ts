import { Injectable, signal } from '@angular/core';
import { ApiError } from '../../shared/models/api-error.models';
import {
  Difficulty,
  OptionLabel,
  QuizResponse,
  QuizResults,
  SubmittedAnswer,
} from '../../shared/models/quiz.models';

@Injectable({ providedIn: 'root' })
export class QuizSessionService {
  readonly quiz = signal<QuizResponse | null>(null);
  readonly results = signal<QuizResults | null>(null);
  readonly answers = signal<Record<string, OptionLabel>>({});
  readonly sourceText = signal('');
  readonly topicContext = signal('');
  readonly difficulty = signal<Difficulty>('medium');
  readonly error = signal<ApiError | null>(null);

  startQuiz(
    quiz: QuizResponse,
    sourceText: string,
    topicContext: string,
    difficulty: Difficulty,
  ) {
    this.quiz.set(quiz);
    this.sourceText.set(sourceText);
    this.topicContext.set(topicContext);
    this.difficulty.set(difficulty);
    this.answers.set({});
    this.results.set(null);
    this.error.set(null);
  }

  selectAnswer(questionId: string, option: OptionLabel) {
    this.answers.update((answers) => ({ ...answers, [questionId]: option }));
  }

  getSubmittedAnswers(): SubmittedAnswer[] {
    return Object.entries(this.answers()).map(([questionId, selectedOption]) => ({
      questionId,
      selectedOption,
    }));
  }

  complete(results: QuizResults) {
    this.results.set(results);
    this.error.set(null);
  }

  replaceQuiz(quiz: QuizResponse) {
    this.quiz.set(quiz);
    this.answers.set({});
    this.results.set(null);
    this.error.set(null);
  }

  setError(error: ApiError) {
    this.error.set(error);
  }
}
