import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { QuizRunComponent } from './features/quiz/quiz-run/quiz-run.component';
import { QuizResultsComponent } from './features/quiz/quiz-results/quiz-results.component';
import { SourceEntryComponent } from './features/quiz/source-entry/source-entry.component';
import { QuizSessionService } from './features/quiz/quiz-session.service';

@Component({
  selector: 'app-root',
  imports: [CommonModule, SourceEntryComponent, QuizRunComponent, QuizResultsComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly session = inject(QuizSessionService);
}
