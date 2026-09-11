import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { QuizApiService } from '../../../core/api/quiz-api.service';
import { QuizSessionService } from '../quiz-session.service';
import { Difficulty } from '../../../shared/models/quiz.models';
import { ApiError } from '../../../shared/models/api-error.models';

@Component({
  selector: 'app-source-entry',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './source-entry.component.html',
  styleUrl: './source-entry.component.scss',
})
export class SourceEntryComponent {
  private readonly api = inject(QuizApiService);
  private readonly session = inject(QuizSessionService);

  readonly topicContext = signal('');
  readonly sourceText = signal('');
  readonly difficulty = signal<Difficulty>('medium');
  readonly loading = signal(false);
  readonly error = signal<ApiError | null>(null);

  get sourceLength() {
    return this.sourceText().replace(/\s/g, '').length;
  }

  setDifficulty(value: Difficulty) {
    this.difficulty.set(value);
  }

  generate() {
    if (this.sourceLength < 100) {
      this.error.set({
        code: 'SOURCE_TEXT_TOO_SHORT',
        message: 'Add at least 100 non-whitespace characters to generate a quiz.',
      });
      return;
    }

    this.loading.set(true);
    this.error.set(null);
    this.api
      .generateQuiz({
        topicContext: this.topicContext().trim() || undefined,
        sourceText: this.sourceText(),
        difficulty: this.difficulty(),
      })
      .subscribe({
        next: (quiz) => {
          this.session.startQuiz(
            quiz,
            this.sourceText(),
            this.topicContext(),
            this.difficulty(),
          );
          this.loading.set(false);
        },
        error: (error: ApiError) => {
          this.error.set(error);
          this.loading.set(false);
        },
      });
  }
}
