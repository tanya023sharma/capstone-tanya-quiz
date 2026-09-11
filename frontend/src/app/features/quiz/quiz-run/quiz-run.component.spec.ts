import { provideHttpClient } from '@angular/common/http';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { QuizApiService } from '../../../core/api/quiz-api.service';
import { QuizResponse, QuizResults } from '../../../shared/models/quiz.models';
import { QuizRunComponent } from './quiz-run.component';
import { QuizSessionService } from '../quiz-session.service';

const quizFixture: QuizResponse = {
  id: 'quiz-1',
  difficulty: 'easy',
  expiresAt: '2099-01-01T00:00:00Z',
  questions: Array.from({ length: 10 }, (_, index) => ({
    id: `question-${index}`,
    number: index + 1,
    prompt: `Question ${index + 1}`,
    options: [
      { label: 'A', text: 'A' },
      { label: 'B', text: 'B' },
      { label: 'C', text: 'C' },
      { label: 'D', text: 'D' },
    ],
  })),
};

const resultFixture: QuizResults = {
  quizId: 'quiz-1',
  score: 10,
  totalQuestions: 10,
  results: [],
};

class QuizApiStub {
  submitQuiz() {
    return of(resultFixture);
  }
}

describe('QuizRunComponent', () => {
  let fixture: ComponentFixture<QuizRunComponent>;
  let component: QuizRunComponent;
  let session: QuizSessionService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [QuizRunComponent],
      providers: [
        provideHttpClient(),
        QuizSessionService,
        { provide: QuizApiService, useClass: QuizApiStub },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(QuizRunComponent);
    component = fixture.componentInstance;
    session = TestBed.inject(QuizSessionService);
    session.startQuiz(quizFixture, 'source', '', 'easy');
    fixture.detectChanges();
  });

  it('preserves selected answers and prevents incomplete submission', () => {
    component.selectAnswer('question-0', 'B');
    component.submit();

    expect(session.answers()['question-0']).toBe('B');
    expect(component.error()?.code).toBe('INCOMPLETE_SUBMISSION');
  });

  it('submits all answers and stores server results', () => {
    for (const question of quizFixture.questions) {
      component.selectAnswer(question.id, 'A');
    }

    component.submit();

    expect(session.results()?.score).toBe(10);
  });
});
