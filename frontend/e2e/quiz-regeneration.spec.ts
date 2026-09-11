import { expect, test } from '@playwright/test';
import { generatedQuizFixture, quizResultsFixture, validSourceText } from './fixtures/quiz-fixtures';

test('regenerates from the same source and clears previous results', async ({ page }) => {
  await page.route('**/api/v1/quizzes', async (route) => {
    await route.fulfill({ status: 201, contentType: 'application/json', body: JSON.stringify(generatedQuizFixture) });
  });
  await page.route('**/api/v1/quizzes/*/submissions', async (route) => {
    await route.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify(quizResultsFixture) });
  });
  await page.route('**/api/v1/quizzes/*/regenerations', async (route) => {
    await route.fulfill({
      status: 201,
      contentType: 'application/json',
      body: JSON.stringify({ ...generatedQuizFixture, id: '00000000-0000-0000-0000-000000000099' }),
    });
  });

  await page.goto('/');
  await page.locator('#source').fill(validSourceText);
  await page.getByRole('button', { name: /Generate 10 questions/i }).click();
  for (let index = 0; index < 10; index += 1) {
    await page.locator('.question-card').nth(index).getByRole('button').first().click();
  }
  await page.getByRole('button', { name: /Submit quiz/i }).click();
  await page.getByRole('button', { name: /Regenerate from the same source/i }).click();

  await expect(page.locator('.question-card')).toHaveCount(10);
  await expect(page.locator('.score-stamp')).toHaveCount(0);
});
