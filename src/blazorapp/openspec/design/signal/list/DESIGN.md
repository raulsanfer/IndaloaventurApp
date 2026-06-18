---
name: Vibrant Modernity
colors:
  surface: '#fbf8ff'
  surface-dim: '#dad9e3'
  surface-bright: '#fbf8ff'
  surface-container-lowest: '#ffffff'
  surface-container-low: '#f4f2fd'
  surface-container: '#eeedf7'
  surface-container-high: '#e8e7f1'
  surface-container-highest: '#e3e1ec'
  on-surface: '#1a1b22'
  on-surface-variant: '#564334'
  inverse-surface: '#2f3038'
  inverse-on-surface: '#f1effa'
  outline: '#897362'
  outline-variant: '#ddc1ae'
  surface-tint: '#904d00'
  primary: '#904d00'
  on-primary: '#ffffff'
  primary-container: '#ff8c00'
  on-primary-container: '#623200'
  inverse-primary: '#ffb77d'
  secondary: '#5f5e5e'
  on-secondary: '#ffffff'
  secondary-container: '#e2dfde'
  on-secondary-container: '#636262'
  tertiary: '#5d5e60'
  on-tertiary: '#ffffff'
  tertiary-container: '#a9aaab'
  on-tertiary-container: '#3d3f40'
  error: '#ba1a1a'
  on-error: '#ffffff'
  error-container: '#ffdad6'
  on-error-container: '#93000a'
  primary-fixed: '#ffdcc3'
  primary-fixed-dim: '#ffb77d'
  on-primary-fixed: '#2f1500'
  on-primary-fixed-variant: '#6e3900'
  secondary-fixed: '#e5e2e1'
  secondary-fixed-dim: '#c8c6c5'
  on-secondary-fixed: '#1c1b1b'
  on-secondary-fixed-variant: '#474746'
  tertiary-fixed: '#e2e2e3'
  tertiary-fixed-dim: '#c6c6c7'
  on-tertiary-fixed: '#1a1c1d'
  on-tertiary-fixed-variant: '#454748'
  background: '#fbf8ff'
  on-background: '#1a1b22'
  surface-variant: '#e3e1ec'
typography:
  headline-xl:
    fontFamily: Outfit
    fontSize: 48px
    fontWeight: '700'
    lineHeight: 56px
    letterSpacing: -0.02em
  headline-lg:
    fontFamily: Outfit
    fontSize: 32px
    fontWeight: '600'
    lineHeight: 40px
    letterSpacing: -0.01em
  headline-lg-mobile:
    fontFamily: Outfit
    fontSize: 28px
    fontWeight: '600'
    lineHeight: 36px
  headline-md:
    fontFamily: Outfit
    fontSize: 24px
    fontWeight: '600'
    lineHeight: 32px
  body-lg:
    fontFamily: Outfit
    fontSize: 18px
    fontWeight: '400'
    lineHeight: 28px
  body-md:
    fontFamily: Outfit
    fontSize: 16px
    fontWeight: '400'
    lineHeight: 24px
  label-lg:
    fontFamily: Outfit
    fontSize: 14px
    fontWeight: '600'
    lineHeight: 20px
    letterSpacing: 0.02em
  label-sm:
    fontFamily: Outfit
    fontSize: 12px
    fontWeight: '500'
    lineHeight: 16px
    letterSpacing: 0.04em
rounded:
  sm: 0.25rem
  DEFAULT: 0.5rem
  md: 0.75rem
  lg: 1rem
  xl: 1.5rem
  full: 9999px
spacing:
  base: 8px
  xs: 4px
  sm: 12px
  md: 24px
  lg: 40px
  xl: 64px
  gutter: 24px
  margin-mobile: 16px
  margin-desktop: 48px
---

## Brand & Style

The design system is built to evoke a sense of high-energy precision and modern adventure. It targets a demographic that values speed, clarity, and bold aesthetics—blending the utility of a technical tool with the warmth of an approachable lifestyle brand.

The chosen style is **Modern / Corporate-Atheletic**. It utilizes high-contrast accents against a clean, structured canvas. The aesthetic is defined by expansive whitespace, crisp geometric typography, and a "Digital-First" energy. Every interaction is designed to feel responsive and decisive, avoiding unnecessary fluff in favor of impactful visual cues that guide the user toward action.

## Colors

The palette is anchored by a high-visibility **Signal Orange** (#FF8C00). This color is the primary driver of the visual hierarchy, reserved strictly for interactive elements, progress indicators, and critical brand moments. 

To maintain a premium feel, the primary orange is balanced by a "Deep Carbon" secondary and a suite of "Technical Grays." The background uses a pure white base to ensure the orange remains vibrant without feeling overwhelming. Success, warning, and error states should utilize semantic variations that match the saturation of the primary orange to maintain a cohesive visual weight across the interface.

## Typography

The typography system relies exclusively on **Outfit**, a geometric sans-serif that strikes a balance between professional structure and friendly curves. 

Headlines use tighter letter spacing and heavier weights to create a strong "editorial" feel, ensuring that key messages command attention. Body text is set with generous line heights to maximize readability during long-form consumption. For labels and utility text, use medium to bold weights with slight tracking (letter-spacing) to ensure legibility at smaller scales, particularly in data-dense environments.

## Layout & Spacing

This design system employs a **Fluid Grid** model based on an 8px square rhythm. This ensures mathematical harmony across all components and screen sizes.

- **Desktop:** 12-column grid with 24px gutters. Max-width container of 1440px.
- **Tablet:** 8-column grid with 20px gutters.
- **Mobile:** 4-column grid with 16px gutters and 16px side margins.

Horizontal spacing between logical groups should use 'lg' (40px) units, while internal component spacing should stick to 'sm' (12px) or 'md' (24px) to maintain a tight, organized appearance.

## Elevation & Depth

Visual hierarchy is achieved through a combination of **Tonal Layering** and **Ambient Shadows**. 

The system avoids heavy, dark shadows in favor of "Soft Depth." Elevate interactive surfaces (like cards or menus) using a low-opacity shadow tinted with the secondary color (e.g., 8% opacity of #1A1A1A). Surfaces should feel like they are floating just above the background. Use "Ghost Outlines" (1px solid borders in a light gray) for inactive elements to keep the UI grounded without adding visual noise. When an element is "active" or "focused," replace the ghost outline with a 2px stroke of the primary orange.

## Shapes

The shape language is consistently **Rounded**. A base radius of 0.5rem (8px) is applied to all standard components like buttons, input fields, and small containers. 

Larger containers, such as cards or modals, should utilize `rounded-lg` (16px) or `rounded-xl` (24px) to create a soft, modern silhouette that contrasts against the sharp geometry of the Outfit typeface. Interactive icons should be housed within circular or highly rounded containers to signify their touch-friendliness.

## Components

### Buttons
- **Primary:** Solid #FF8C00 fill with White text. Bold weight. High-contrast hover state (slight darken).
- **Secondary:** Transparent background with a 2px #FF8C00 stroke. 
- **Tertiary:** Text-only in #FF8C00 with an underline or icon suffix.

### Cards
Cards use a white background, `rounded-lg` corners, and a soft ambient shadow. If a card is the "TrailSignal" variety, it may use the primary orange as a background with white text for maximum impact.

### Input Fields
Fields feature a 1px light gray border that transitions to a 2px #FF8C00 border on focus. Labels sit outside the field in `label-lg` typography.

### Chips & Tags
Small, highly rounded (`rounded-xl`) elements. Default state is a light gray fill; active/selected state uses the primary orange fill with white text.

### Icons
All functional icons (navigation, actions) must use the primary orange color or be paired with an orange background to maintain the "interactive" visual cue. Use a consistent stroke weight (1.5px or 2px) to match the typography's visual density.