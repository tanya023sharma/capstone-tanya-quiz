export const validSourceText =
  'The water cycle moves water through evaporation, condensation, precipitation, and collection. Heat from the sun causes liquid water to evaporate into water vapor. As vapor cools, it condenses into clouds. Water returns to Earth as precipitation and collects in bodies of water.';

export const generatedQuizFixture = {
  id: '00000000-0000-0000-0000-000000000001',
  difficulty: 'easy',
  questions: Array.from({ length: 10 }, (_, index) => ({
    id: `00000000-0000-0000-0000-00000000000${index + 2}`,
    number: index + 1,
    prompt: `Which statement is supported by the source? ${index + 1}`,
    options: [
      { label: 'A', text: 'Evaporation moves water into vapor.' },
      { label: 'B', text: 'The source says nothing about water.' },
      { label: 'C', text: 'The source rejects condensation.' },
      { label: 'D', text: 'A detail unrelated to the water cycle.' },
    ],
  })),
  expiresAt: '2099-01-01T00:00:00Z',
};

export const quizResultsFixture = {
  quizId: generatedQuizFixture.id,
  score: 7,
  totalQuestions: 10,
  results: generatedQuizFixture.questions.map((question, index) => ({
    questionId: question.id,
    number: question.number,
    prompt: question.prompt,
    selectedOption: 'A',
    correctOption: 'A',
    isCorrect: index < 7,
    options: question.options.map((option) => ({
      ...option,
      isCorrect: option.label === 'A',
      explanation:
        option.label === 'A'
          ? 'This is supported by the source material.'
          : 'This option is not supported by the source material.',
    })),
    evidence: [
      {
        quote: 'The water cycle moves water through evaporation, condensation, precipitation, and collection.',
        sourceStart: 0,
        sourceEnd: 99,
      },
    ],
  })),
};
