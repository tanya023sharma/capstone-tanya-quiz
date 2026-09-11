import { provideHttpClient } from '@angular/common/http';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SourceEntryComponent } from './source-entry.component';

describe('SourceEntryComponent', () => {
  let fixture: ComponentFixture<SourceEntryComponent>;
  let component: SourceEntryComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SourceEntryComponent],
      providers: [provideHttpClient()],
    }).compileComponents();

    fixture = TestBed.createComponent(SourceEntryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('keeps generation disabled until the source reaches the minimum length', () => {
    component.sourceText.set('short');
    fixture.detectChanges();

    expect(component.sourceLength).toBe(5);
    expect(fixture.nativeElement.querySelector('.primary-action').disabled).toBe(true);
  });

  it('reports a local validation error for short source text', () => {
    component.sourceText.set('short');

    component.generate();

    expect(component.error()?.code).toBe('SOURCE_TEXT_TOO_SHORT');
  });
});
