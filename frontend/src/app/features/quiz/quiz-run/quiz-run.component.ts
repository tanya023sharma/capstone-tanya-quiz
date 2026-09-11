import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { QuizApiService } from '../../../core/api/quiz-api.service';
import { ApiError } from '../../../shared/models/api-error.models';
import { OptionLabel } from '../../../shared/models/quiz.models';
import { QuizSessionService } from '../quiz-session.service';

@Component({
  selector: 'app-quiz-run',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './quiz-run.component.html',
  styleUrl: './quiz-run.component.scss',
})
export class QuizRunComponent {
  private readonly api = inject(QuizApiService);
  readonly session = inject(QuizSessionService);
  readonly loading = signal(false);
  readonly error = signal<ApiError | null>(null);

  readonly quiz = computed(() => this.session.quiz());
  readonly answeredCount = computed(() => Object.keys(this.session.answers()).length);
  readonly allAnswered = computed(() => this.answeredCount() === 10);

  selectAnswer(questionId: string, option: OptionLabel) {
    this.session.selectAnswer(questionId, option);
    this.error.set(null);
  }

  submit() {
    const quiz = this.quiz();
    if (!quiz) {
      return;
    }

    if (!this.allAnswered()) {
      this.error.set({
        code: 'INCOMPLETE_SUBMISSION',
        message: `Answer all 10 questions before submitting. ${10 - this.answeredCount()} remain.`,
      });
      return;
    }

    this.loading.set(true);
    this.error.set(null);
    this.api.submitQuiz(quiz.id, this.session.getSubmittedAnswers()).subscribe({
      next: (results) => {
        this.session.complete(results);
        this.loading.set(false);
      },
      error: (error: ApiError) => {
        this.error.set(error);
        this.loading.set(false);
      },
    });
  }
}
