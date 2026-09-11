import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { QuizApiService } from '../../../core/api/quiz-api.service';
import { ApiError } from '../../../shared/models/api-error.models';
import { QuizSessionService } from '../quiz-session.service';

@Component({
  selector: 'app-quiz-results',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './quiz-results.component.html',
  styleUrl: './quiz-results.component.scss',
})
export class QuizResultsComponent {
  private readonly api = inject(QuizApiService);
  readonly session = inject(QuizSessionService);
  readonly loading = signal(false);
  readonly error = signal<ApiError | null>(null);
  readonly results = computed(() => this.session.results());
  readonly percentage = computed(() => {
    const results = this.results();
    return results ? Math.round((results.score / results.totalQuestions) * 100) : 0;
  });

  regenerate() {
    const quiz = this.session.quiz();
    if (!quiz) {
      return;
    }

    this.loading.set(true);
    this.error.set(null);
    this.api.regenerateQuiz(quiz.id).subscribe({
      next: (replacement) => {
        this.session.replaceQuiz(replacement);
        this.loading.set(false);
      },
      error: (error: ApiError) => {
        this.error.set(error);
        this.loading.set(false);
      },
    });
  }
}
