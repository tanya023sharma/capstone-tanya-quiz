import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import {
  GenerateQuizRequest,
  QuizResponse,
  QuizResults,
  SubmittedAnswer,
} from '../../shared/models/quiz.models';

@Injectable({ providedIn: 'root' })
export class QuizApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/v1/quizzes';

  generateQuiz(request: GenerateQuizRequest) {
    return this.http.post<QuizResponse>(this.baseUrl, request);
  }

  submitQuiz(quizId: string, answers: SubmittedAnswer[]) {
    return this.http.post<QuizResults>(`${this.baseUrl}/${quizId}/submissions`, {
      answers,
    });
  }

  regenerateQuiz(quizId: string) {
    return this.http.post<QuizResponse>(
      `${this.baseUrl}/${quizId}/regenerations`,
      {},
    );
  }
}
