import { provideHttpClient } from '@angular/common/http';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { QuizApiService } from '../../../core/api/quiz-api.service';
import { QuizResultsComponent } from './quiz-results.component';
import { QuizSessionService } from '../quiz-session.service';

class QuizApiStub {
  regenerateQuiz() {
    return of({ id: 'replacement', difficulty: 'easy' as const, expiresAt: '', questions: [] });
  }
}

describe('Quiz regeneration', () => {
  let fixture: ComponentFixture<QuizResultsComponent>;
  let component: QuizResultsComponent;
  let session: QuizSessionService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [QuizResultsComponent],
      providers: [
        provideHttpClient(),
        QuizSessionService,
        { provide: QuizApiService, useClass: QuizApiStub },
      ],
    }).compileComponents();
    fixture = TestBed.createComponent(QuizResultsComponent);
    component = fixture.componentInstance;
    session = TestBed.inject(QuizSessionService);
    session.startQuiz({ id: 'original', difficulty: 'easy', expiresAt: '', questions: [] }, 'source', '', 'easy');
    fixture.detectChanges();
  });

  it('replaces the active quiz and clears the result state', () => {
    component.regenerate();

    expect(session.quiz()?.id).toBe('replacement');
    expect(session.results()).toBeNull();
  });
});
