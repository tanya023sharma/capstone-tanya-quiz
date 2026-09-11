import { provideHttpClient } from '@angular/common/http';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { QuizApiService } from '../../../core/api/quiz-api.service';
import { QuizResponse, QuizResults } from '../../../shared/models/quiz.models';
import { QuizResultsComponent } from './quiz-results.component';
import { QuizSessionService } from '../quiz-session.service';

const quizFixture: QuizResponse = {
  id: 'quiz-1',
  difficulty: 'medium',
  expiresAt: '2099-01-01T00:00:00Z',
  questions: [],
};

const resultsFixture: QuizResults = {
  quizId: 'quiz-1',
  score: 8,
  totalQuestions: 10,
  results: [
    {
      questionId: 'question-1',
      number: 1,
      prompt: 'Question',
      selectedOption: 'A',
      correctOption: 'B',
      isCorrect: false,
      options: [
        { label: 'A', text: 'A', isCorrect: false, explanation: 'Not supported.' },
        { label: 'B', text: 'B', isCorrect: true, explanation: 'Supported.' },
        { label: 'C', text: 'C', isCorrect: false, explanation: 'Not supported.' },
        { label: 'D', text: 'D', isCorrect: false, explanation: 'Not supported.' },
      ],
      evidence: [{ quote: 'Source quote', sourceStart: 0, sourceEnd: 12 }],
    },
  ],
};

class QuizApiStub {
  regenerateQuiz() {
    return of(quizFixture);
  }
}

describe('QuizResultsComponent', () => {
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
    session.startQuiz(quizFixture, 'source', '', 'medium');
    session.complete(resultsFixture);
    fixture.detectChanges();
  });

  it('renders the score, four explanations, and source evidence', () => {
    expect(component.percentage()).toBe(80);
    expect(fixture.nativeElement.querySelectorAll('.explanation').length).toBe(4);
    expect(fixture.nativeElement.querySelector('.evidence')).toBeTruthy();
  });

  it('regenerates and clears the previous result state', () => {
    component.regenerate();

    expect(session.quiz()?.id).toBe('quiz-1');
    expect(session.results()).toBeNull();
  });
});
