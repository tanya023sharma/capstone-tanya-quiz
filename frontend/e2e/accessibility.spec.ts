import { expect, test } from '@playwright/test';

test('source entry exposes labelled controls and keyboard-focusable actions', async ({ page }) => {
  await page.goto('/');

  await expect(page.getByLabel('Topic context optional')).toBeVisible();
  await expect(page.getByLabel('Your source text')).toBeVisible();
  await expect(page.getByRole('group', { name: 'Difficulty' })).toBeVisible();
  await expect(page.getByRole('button', { name: /Generate 10 questions/i })).toBeVisible();

  await page.keyboard.press('Tab');
  await expect(page.locator(':focus')).toBeVisible();
});
