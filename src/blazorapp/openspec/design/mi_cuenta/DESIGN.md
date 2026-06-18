---
name: Indalo Vista
colors:
  surface: '#fafaf5'
  surface-dim: '#dadad5'
  surface-bright: '#fafaf5'
  surface-container-lowest: '#ffffff'
  surface-container-low: '#f4f4ef'
  surface-container: '#eeeee9'
  surface-container-high: '#e8e8e4'
  surface-container-highest: '#e2e3de'
  on-surface: '#1a1c19'
  on-surface-variant: '#564334'
  inverse-surface: '#2f312e'
  inverse-on-surface: '#f1f1ec'
  outline: '#897362'
  outline-variant: '#ddc1ae'
  surface-tint: '#904d00'
  primary: '#904d00'
  on-primary: '#ffffff'
  primary-container: '#ff8c00'
  on-primary-container: '#623200'
  inverse-primary: '#ffb77d'
  secondary: '#5b6400'
  on-secondary: '#ffffff'
  secondary-container: '#dce87b'
  on-secondary-container: '#5f6801'
  tertiary: '#5f5e5e'
  on-tertiary: '#ffffff'
  tertiary-container: '#aba9a9'
  on-tertiary-container: '#3f3e3e'
  error: '#ba1a1a'
  on-error: '#ffffff'
  error-container: '#ffdad6'
  on-error-container: '#93000a'
  primary-fixed: '#ffdcc3'
  primary-fixed-dim: '#ffb77d'
  on-primary-fixed: '#2f1500'
  on-primary-fixed-variant: '#6e3900'
  secondary-fixed: '#dfea7d'
  secondary-fixed-dim: '#c3ce65'
  on-secondary-fixed: '#1a1e00'
  on-secondary-fixed-variant: '#444b00'
  tertiary-fixed: '#e5e2e1'
  tertiary-fixed-dim: '#c8c6c5'
  on-tertiary-fixed: '#1c1b1b'
  on-tertiary-fixed-variant: '#474746'
  background: '#fafaf5'
  on-background: '#1a1c19'
  surface-variant: '#e2e3de'
typography:
  display-lg:
    fontFamily: Outfit
    fontSize: 56px
    fontWeight: '700'
    lineHeight: '1.1'
    letterSpacing: -0.02em
  headline-lg:
    fontFamily: Outfit
    fontSize: 32px
    fontWeight: '600'
    lineHeight: '1.2'
  headline-lg-mobile:
    fontFamily: Outfit
    fontSize: 28px
    fontWeight: '600'
    lineHeight: '1.2'
  headline-md:
    fontFamily: Outfit
    fontSize: 24px
    fontWeight: '600'
    lineHeight: '1.3'
  body-lg:
    fontFamily: Outfit
    fontSize: 18px
    fontWeight: '400'
    lineHeight: '1.6'
  body-md:
    fontFamily: Outfit
    fontSize: 16px
    fontWeight: '400'
    lineHeight: '1.6'
  label-md:
    fontFamily: Outfit
    fontSize: 14px
    fontWeight: '600'
    lineHeight: '1.4'
    letterSpacing: 0.05em
rounded:
  sm: 0.25rem
  DEFAULT: 0.5rem
  md: 0.75rem
  lg: 1rem
  xl: 1.5rem
  full: 9999px
spacing:
  unit: 4px
  gutter: 24px
  margin-mobile: 20px
  margin-desktop: 64px
  container-max: 1280px
---

## Brand & Style

This design system is built for the adventurous spirit, capturing the raw energy of mountaineering and outdoor exploration. It utilizes a **High-Contrast / Modern** style that prioritizes immediate legibility and high-impact visual hierarchy. The brand personality is rugged yet refined—reliable enough for technical data but energetic enough to inspire action.

The aesthetic avoids delicate or fragile elements, instead opting for sturdy strokes, clear boundaries, and substantial whitespace to reflect the openness of the outdoors. Visual cues are taken from high-performance outdoor gear: functional, bold, and unapologetically visible.

## Colors

The palette is anchored by a high-visibility **Vibrant Orange**, pulled directly from the core of the club's identity. This color is reserved for primary actions and critical information. A natural **Olive Green** serves as the secondary accent, providing a grounded, earth-toned counterpoint that links the UI to the environment.

The background uses a slightly warm off-white (`#F7F7F2`) to reduce eye strain in outdoor lighting conditions, while a deep charcoal (`#1A1A1A`) provides the necessary contrast for text and structural borders.

## Typography

The design system exclusively utilizes **Outfit**, a geometric sans-serif with a friendly yet technical character. Its rounded terminals mirror the "Indalo" logo's geometry and ensure the high-contrast layout feels approachable.

Headlines use heavy weights and tighter tracking to command attention, while body text maintains a generous line height for maximum readability during physical activity or on the move. Labels are set in uppercase with increased letter spacing to differentiate functional metadata from narrative content.

## Layout & Spacing

The layout follows a **Fluid Grid** model with a strict 4px baseline. A 12-column system is used for desktop environments, transitioning to a 4-column system for mobile devices. 

Spacing is intentional and generous, favoring a "breathable" feel that prevents information overload. Margins are kept wide to ensure content is centered and easy to scan. 
- **Mobile:** 20px margins, 16px gutters.
- **Tablet:** 40px margins, 20px gutters.
- **Desktop:** 64px margins, 24px gutters, with a maximum container width of 1280px to maintain comfortable line lengths.

## Elevation & Depth

To maintain the rugged, functional aesthetic, this design system avoids soft, floating shadows. Instead, it uses **Tonal Layers** and **Bold Outlines**.

Depth is communicated through:
1.  **Surface Tiers:** Neutral backgrounds elevate to pure white surfaces for cards and modals.
2.  **Hard Strokes:** A 1.5px or 2px border in charcoal or olive green is used to define containers, mimicking the stitched seams and reinforced edges of mountain gear.
3.  **Flat Overlays:** Modals and menus use a high-opacity (80%) charcoal scrim to bring focus to the foreground without losing context.

## Shapes

The shape language is consistently **Rounded**, using a 0.5rem base radius. This softens the high-contrast color palette and aligns with the circular nature of the club logo and the Outfit typeface.

- Base elements (Inputs, Small Buttons): 0.5rem (8px)
- Large elements (Cards, Hero sections): 1rem (16px)
- Interactive accents (Chips, Tags): Full pill-shape for maximum distinction.

## Components

### Buttons
Primary buttons are solid Vibrant Orange with Charcoal text for maximum contrast. Secondary buttons use an Olive Green outline with a 2px stroke. Buttons should have a minimum touch target of 48px in height.

### Input Fields
Inputs feature a 1.5px Charcoal border that thickens to 2px and changes to Vibrant Orange on focus. Labels sit clearly above the field, never as placeholder text.

### Cards
Cards are white with a subtle 1px border (`#E0E0DB`). They do not use shadows; instead, they use a "lift" effect on hover where the border color shifts to Olive Green.

### Chips & Tags
Used for trail difficulty or equipment categories. These use the Pill-shape (Rounded-xl) with background colors that correspond to the category (e.g., Green for "Easy", Orange for "Advanced").

### Lists
Lists use generous vertical padding (16px) and are separated by thin horizontal rules. Icons within lists are always enclosed in a small rounded-lg square with a light olive background.