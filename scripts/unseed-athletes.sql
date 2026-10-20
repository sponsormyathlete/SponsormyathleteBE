-- Removes the 4 seed athletes from scripts/seed-athletes.sql (cascades to their
-- accomplishments, photos, and sponsorship packages via FK ON DELETE CASCADE).
DELETE FROM "Users" WHERE "Auth0Sub" IN ('seed|athlete-1', 'seed|athlete-2', 'seed|athlete-3', 'seed|athlete-4');
