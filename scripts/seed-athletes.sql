-- Temporary seed data: 4 published athlete profiles for local testing/demo.
-- Fake Auth0Sub values ("seed|...") so they're obviously not real accounts and can't log in.

BEGIN;

-- 1. Jess "The Hammer" Nguyen — Boxing, District level, QLD
INSERT INTO "Users" ("Id","Auth0Sub","Email","Role","IsFlagged","BlockedAttemptCount","CreatedAt")
VALUES ('11111111-1111-1111-1111-111111111101','seed|athlete-1','jess.seed@example.com','Athlete',false,0,now());

INSERT INTO "AthleteProfiles" ("Id","UserId","DisplayName","Sport","CompetitiveLevel","Organisation","State",
  "DateOfBirth","Gender","WeightClass","Bio","InstagramHandle","FollowerCount","AverageReach",
  "VerificationReference","VerificationStatus","VerifiedAt","SubscriptionStatus","CreatedAt","UpdatedAt")
VALUES ('11111111-1111-1111-1111-111111111102','11111111-1111-1111-1111-111111111101',
  'Jess "The Hammer" Nguyen','Boxing','District','Toowoomba Boxing Club','QLD',
  '1999-04-12','Female','Welterweight (66.7kg)',
  'I have been boxing out of Toowoomba for six years and just picked up the district title this season. Training six days a week around a full-time job, chasing a state qualifier spot next. I bring a loud local following to every card and love giving sponsors real fight-night visibility, not just a logo nobody notices. Looking for gear, supplement, or recovery partners who want to grow with me as I push toward state and national level.',
  'jess_hammer_box',4200,1800,
  'Reference available from head coach at Toowoomba Boxing Club.','Published',now(),'Trialing',now(),now());

INSERT INTO "AthleteAccomplishments" ("Id","AthleteProfileId","Description","Year") VALUES
  ('11111111-1111-1111-1111-111111111103','11111111-1111-1111-1111-111111111102','District Welterweight Champion',2025),
  ('11111111-1111-1111-1111-111111111104','11111111-1111-1111-1111-111111111102','Queensland Golden Gloves Semi-finalist',2024);

INSERT INTO "AthletePhotos" ("Id","AthleteProfileId","Url","IsProfilePhoto","UploadedAt") VALUES
  ('11111111-1111-1111-1111-111111111105','11111111-1111-1111-1111-111111111102','https://i.pravatar.cc/600?img=47',true,now()),
  ('11111111-1111-1111-1111-111111111106','11111111-1111-1111-1111-111111111102','https://picsum.photos/seed/jessbox1/800/600',false,now());

INSERT INTO "SponsorshipPackages" ("Id","AthleteProfileId","Title","Description","CreatedAt") VALUES
  ('11111111-1111-1111-1111-111111111107','11111111-1111-1111-1111-111111111102','Fight-night kit branding',
   'Logo on singlet and shorts for every card this season, plus a shoutout in my post-fight reel.',now());

-- 2. Tyrell Watson — Brazilian Jiu-Jitsu, State level, NSW
INSERT INTO "Users" ("Id","Auth0Sub","Email","Role","IsFlagged","BlockedAttemptCount","CreatedAt")
VALUES ('22222222-2222-2222-2222-222222222201','seed|athlete-2','tyrell.seed@example.com','Athlete',false,0,now());

INSERT INTO "AthleteProfiles" ("Id","UserId","DisplayName","Sport","CompetitiveLevel","Organisation","State",
  "DateOfBirth","Gender","WeightClass","Bio","InstagramHandle","FollowerCount","AverageReach",
  "VerificationReference","VerificationStatus","VerifiedAt","SubscriptionStatus","CreatedAt","UpdatedAt")
VALUES ('22222222-2222-2222-2222-222222222202','22222222-2222-2222-2222-222222222201',
  'Tyrell Watson','Brazilian Jiu-Jitsu (BJJ)','State','Sydney BJJ Alliance','NSW',
  '1996-09-03','Male','Lightweight (76kg)',
  'Purple belt out of Sydney BJJ Alliance, currently ranked top three in the state gi division. I post weekly technique breakdowns that regularly outperform my follower count, so sponsors get real engagement, not just impressions. Training full-time toward brown belt and a crack at the Oceania trials. Open to gi, rashguard, and supplement partnerships that want consistent, high-quality content.',
  'tyrell_bjj',8900,3200,
  'Reference available from head coach at Sydney BJJ Alliance.','Published',now(),'Trialing',now(),now());

INSERT INTO "AthleteAccomplishments" ("Id","AthleteProfileId","Description","Year") VALUES
  ('22222222-2222-2222-2222-222222222203','22222222-2222-2222-2222-222222222202','NSW State Gi Champion — Purple Belt',2025),
  ('22222222-2222-2222-2222-222222222204','22222222-2222-2222-2222-222222222202','3rd place, ADCC Oceania Trials',2024);

INSERT INTO "AthletePhotos" ("Id","AthleteProfileId","Url","IsProfilePhoto","UploadedAt") VALUES
  ('22222222-2222-2222-2222-222222222205','22222222-2222-2222-2222-222222222202','https://i.pravatar.cc/600?img=12',true,now()),
  ('22222222-2222-2222-2222-222222222206','22222222-2222-2222-2222-222222222202','https://picsum.photos/seed/tyrellbjj1/800/600',false,now());

INSERT INTO "SponsorshipPackages" ("Id","AthleteProfileId","Title","Description","CreatedAt") VALUES
  ('22222222-2222-2222-2222-222222222207','22222222-2222-2222-2222-222222222202','Weekly technique reel series',
   'Four Instagram Reels a month featuring your gear, each with a technique breakdown my audience actually watches to the end.',now());

-- 3. Amara Thompson — Athletics (sprints), National level, VIC
INSERT INTO "Users" ("Id","Auth0Sub","Email","Role","IsFlagged","BlockedAttemptCount","CreatedAt")
VALUES ('33333333-3333-3333-3333-333333333301','seed|athlete-3','amara.seed@example.com','Athlete',false,0,now());

INSERT INTO "AthleteProfiles" ("Id","UserId","DisplayName","Sport","CompetitiveLevel","Organisation","State",
  "DateOfBirth","Gender","Bio","InstagramHandle","FollowerCount","AverageReach",
  "VerificationReference","VerificationStatus","VerifiedAt","SubscriptionStatus","CreatedAt","UpdatedAt")
VALUES ('33333333-3333-3333-3333-333333333302','33333333-3333-3333-3333-333333333301',
  'Amara Thompson','Athletics','National','Athletics Victoria','VIC',
  '2001-02-20','Female',
  'National U23 200m champion training out of Melbourne with my sights on a Commonwealth Games call-up. Sprinting is an expensive sport — spikes, physio, and travel to interstate meets add up fast — so I am upfront that sponsorship goes straight back into performance. I share training blocks and race-day build-up content that performs well with a young, engaged audience. Keen to talk to activewear, recovery, and nutrition brands.',
  'amara_sprints',15600,6100,
  'Reference available from Athletics Victoria high-performance coach.','Published',now(),'Trialing',now(),now());

INSERT INTO "AthleteAccomplishments" ("Id","AthleteProfileId","Description","Year") VALUES
  ('33333333-3333-3333-3333-333333333303','33333333-3333-3333-3333-333333333302','National U23 200m Champion',2025),
  ('33333333-3333-3333-3333-333333333304','33333333-3333-3333-3333-333333333302','Victorian Open 100m club record holder',2024);

INSERT INTO "AthletePhotos" ("Id","AthleteProfileId","Url","IsProfilePhoto","UploadedAt") VALUES
  ('33333333-3333-3333-3333-333333333305','33333333-3333-3333-3333-333333333302','https://i.pravatar.cc/600?img=32',true,now()),
  ('33333333-3333-3333-3333-333333333306','33333333-3333-3333-3333-333333333302','https://picsum.photos/seed/amararun1/800/600',false,now());

INSERT INTO "SponsorshipPackages" ("Id","AthleteProfileId","Title","Description","CreatedAt") VALUES
  ('33333333-3333-3333-3333-333333333307','33333333-3333-3333-3333-333333333302','Race-day kit + story takeover',
   'Branded kit at every national-level meet this season, plus an Instagram story takeover from race day.',now());

-- 4. Marcus O'Brien — Cricket, Professional level, WA
INSERT INTO "Users" ("Id","Auth0Sub","Email","Role","IsFlagged","BlockedAttemptCount","CreatedAt")
VALUES ('44444444-4444-4444-4444-444444444401','seed|athlete-4','marcus.seed@example.com','Athlete',false,0,now());

INSERT INTO "AthleteProfiles" ("Id","UserId","DisplayName","Sport","CompetitiveLevel","Organisation","State",
  "DateOfBirth","Gender","Bio","InstagramHandle","FollowerCount","AverageReach",
  "VerificationReference","VerificationStatus","VerifiedAt","SubscriptionStatus","CreatedAt","UpdatedAt")
VALUES ('44444444-4444-4444-4444-444444444402','44444444-4444-4444-4444-444444444401',
  'Marcus O''Brien','Cricket','Professional','Perth Scorchers Academy','WA',
  '1998-07-15','Male',
  'All-rounder on an academy contract with the Perth Scorchers, grinding up from grade cricket over the past four seasons. Bat, bowl, and do the unglamorous overs nobody wants — the kind of player sponsors can build a long relationship with rather than a one-off post. Strong following across WA grade and academy cricket circles, with match-day content that reaches well beyond my own follower count.',
  'marcus_obrien_cricket',22000,9000,
  'Reference available from Perth Scorchers Academy manager.','Published',now(),'Trialing',now(),now());

INSERT INTO "AthleteAccomplishments" ("Id","AthleteProfileId","Description","Year") VALUES
  ('44444444-4444-4444-4444-444444444403','44444444-4444-4444-4444-444444444402','WA Premier Cricket Player of the Season',2025),
  ('44444444-4444-4444-4444-444444444404','44444444-4444-4444-4444-444444444402','WA State Second XI call-up',2024);

INSERT INTO "AthletePhotos" ("Id","AthleteProfileId","Url","IsProfilePhoto","UploadedAt") VALUES
  ('44444444-4444-4444-4444-444444444405','44444444-4444-4444-4444-444444444402','https://i.pravatar.cc/600?img=51',true,now()),
  ('44444444-4444-4444-4444-444444444406','44444444-4444-4444-4444-444444444402','https://picsum.photos/seed/marcuscricket1/800/600',false,now());

INSERT INTO "SponsorshipPackages" ("Id","AthleteProfileId","Title","Description","CreatedAt") VALUES
  ('44444444-4444-4444-4444-444444444407','44444444-4444-4444-4444-444444444402','Bat/kit branding + match-day story',
   'Branded bat and kit for every grade and academy fixture this season, with a match-day Instagram story each round.',now());

COMMIT;
