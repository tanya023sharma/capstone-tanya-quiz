import { expect, test } from '@playwright/test';
import { generatedQuizFixture, validSourceText } from './fixtures/quiz-fixtures';

test('generates a ten-question quiz without exposing answer keys', async ({ page }) => {
  await page.route('**/api/v1/quizzes', async (route) => {
    await route.fulfill({
      status: 201,
      contentType: 'application/json',
      body: JSON.stringify(generatedQuizFixture),
    });
  });

  await page.goto('/');
  await page.locator('#source').fill(validSourceText);
  await page.getByRole('button', { name: /Generate 10 questions/i }).click();

  await expect(page.locator('.question-card')).toHaveCount(10);
  await expect(page.locator('.option-button')).toHaveCount(40);
  await expect(page.locator('text=Correct answer')).toHaveCount(0);
});
