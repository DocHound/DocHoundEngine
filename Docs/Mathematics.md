# Mathematics

Kava Docs supports mathematical equations supporting various standards such as (La)TeX Math markup (for specifics on this standard, see https://en.wikibooks.org/wiki/LaTeX/Mathematics), MathML (see also: https://www.w3.org/Math/), and HTML Math Markup.

> Note: To use KavaDocs Math, the feature has to be enabled in the TOC, either globally, or on a topic-by-topic basis, by setting the **useMathematics** to *true*. This is done to prevent performance problems in repositories and/or topics that do not need this feature. (Note that loading all the features required for LaTeX Math features can be very resource intensive, therefore it should only be activated when it is really used). For more information, see [Table of Contents File Structure](TOC-File-Structure)

> Note: KavaDoc Math features are implemented using MathJax. See also: https://www.mathjax.org/

When KavaDocs Math features are enabled for a topic (or topic tree), the following math features are available:

### HTML Math Markup

Math can be embedded using an HTML `div` tag with a "math" class assigned, like so: `<div class="math">`. Here is a more specific example:

```html
<div class="math">
\begin{equation}
  \int_0^\infty \frac{x^3}{e^x-1}\,dx = \frac{\pi^4}{15}  
\end{equation}
</div>
```

This creates the following output:

<div class="math">
\begin{equation}
  \int_0^\infty \frac{x^3}{e^x-1}\,dx = \frac{\pi^4}{15}  
\end{equation}
</div>

### Inline Expressions

You can use inline expressions using a single `$` to delimit an expression. For instance, consider the following example:

```
Who hasn't heard of $E=mc^2$? 

This is another inline expression:  
$\vec{F} = \frac{d \vec{p}}{dt} = m \frac{d \vec{v}}{dt} = m \vec{a}$
```
**Output:**

Who hasn't heard of $E=mc^2$? 

This is another inline expression:  
$\vec{F} = \frac{d \vec{p}}{dt} = m \frac{d \vec{v}}{dt} = m \vec{a}$

### Block Expressions

You can also use block expressions that use `$$` as delimiters in a block:

```
$$
\frac{n!}{k!(n-k)!} = \binom{n}{k}
$$
```

**Output:**

$$
\frac{n!}{k!(n-k)!} = \binom{n}{k}
$$

### Mixed Inline and Block Expressions

The following is a mixture of inline and block operations:

```
When $a \ne 0$, there are two solutions to $ax^2 + bx + c = 0$ and they are

$$x = {-b \pm \sqrt{b^2-4ac} \over 2a}$$
```

**Output:**

When $a \ne 0$, there are two solutions to $ax^2 + bx + c = 0$ and they are

$$x = {-b \pm \sqrt{b^2-4ac} \over 2a}$$

### More Examples

A much longer expression:

```html
<div class="math">
\begin{align}
\sqrt{37} & = \sqrt{\frac{73^2-1}{12^2}} \\
 & = \sqrt{\frac{73^2}{12^2}\cdot\frac{73^2-1}{73^2}} \\ 
 & = \sqrt{\frac{73^2}{12^2}}\sqrt{\frac{73^2-1}{73^2}} \\
 & = \frac{73}{12}\sqrt{1 - \frac{1}{73^2}} \\ 
 & \approx \frac{73}{12}\left(1 - \frac{1}{2\cdot73^2}\right)
\end{align}
</div>
```

**Output:**

<div class="math">
\begin{align}
\sqrt{37} & = \sqrt{\frac{73^2-1}{12^2}} \\
 & = \sqrt{\frac{73^2}{12^2}\cdot\frac{73^2-1}{73^2}} \\ 
 & = \sqrt{\frac{73^2}{12^2}}\sqrt{\frac{73^2-1}{73^2}} \\
 & = \frac{73}{12}\sqrt{1 - \frac{1}{73^2}} \\ 
 & \approx \frac{73}{12}\left(1 - \frac{1}{2\cdot73^2}\right)
\end{align}
</div>

### MathML

The following is a **MathML** block:

```xml
<math xmlns="http://www.w3.org/1998/Math/MathML" display="block">
  <msup>
    <mrow>
      <mo>(</mo>
      <munderover>
        <mo>&#x2211;<!-- ∑ --></mo>
        <mrow class="MJX-TeXAtom-ORD">
          <mi>k</mi>
          <mo>=</mo>
          <mn>1</mn>
        </mrow>
        <mi>n</mi>
      </munderover>
      <msub>
        <mi>a</mi>
        <mi>k</mi>
      </msub>
      <msub>
        <mi>b</mi>
        <mi>k</mi>
      </msub>
      <mo>)</mo>
    </mrow>
    <mrow class="MJX-TeXAtom-ORD">
      <mspace width="negativethinmathspace" />
      <mspace width="negativethinmathspace" />
      <mn>2</mn>
    </mrow>
  </msup>
  <mo>&#x2264;<!-- ≤ --></mo>
  <mrow>
    <mo>(</mo>
    <munderover>
      <mo>&#x2211;<!-- ∑ --></mo>
      <mrow class="MJX-TeXAtom-ORD">
        <mi>k</mi>
        <mo>=</mo>
        <mn>1</mn>
      </mrow>
      <mi>n</mi>
    </munderover>
    <msubsup>
      <mi>a</mi>
      <mi>k</mi>
      <mn>2</mn>
    </msubsup>
    <mo>)</mo>
  </mrow>
  <mrow>
    <mo>(</mo>
    <munderover>
      <mo>&#x2211;<!-- ∑ --></mo>
      <mrow class="MJX-TeXAtom-ORD">
        <mi>k</mi>
        <mo>=</mo>
        <mn>1</mn>
      </mrow>
      <mi>n</mi>
    </munderover>
    <msubsup>
      <mi>b</mi>
      <mi>k</mi>
      <mn>2</mn>
    </msubsup>
    <mo>)</mo>
  </mrow>
</math>
```

**Output:**

<math xmlns="http://www.w3.org/1998/Math/MathML" display="block">
  <msup>
    <mrow>
      <mo>(</mo>
      <munderover>
        <mo>&#x2211;<!-- ∑ --></mo>
        <mrow class="MJX-TeXAtom-ORD">
          <mi>k</mi>
          <mo>=</mo>
          <mn>1</mn>
        </mrow>
        <mi>n</mi>
      </munderover>
      <msub>
        <mi>a</mi>
        <mi>k</mi>
      </msub>
      <msub>
        <mi>b</mi>
        <mi>k</mi>
      </msub>
      <mo>)</mo>
    </mrow>
    <mrow class="MJX-TeXAtom-ORD">
      <mspace width="negativethinmathspace" />
      <mspace width="negativethinmathspace" />
      <mn>2</mn>
    </mrow>
  </msup>
  <mo>&#x2264;<!-- ≤ --></mo>
  <mrow>
    <mo>(</mo>
    <munderover>
      <mo>&#x2211;<!-- ∑ --></mo>
      <mrow class="MJX-TeXAtom-ORD">
        <mi>k</mi>
        <mo>=</mo>
        <mn>1</mn>
      </mrow>
      <mi>n</mi>
    </munderover>
    <msubsup>
      <mi>a</mi>
      <mi>k</mi>
      <mn>2</mn>
    </msubsup>
    <mo>)</mo>
  </mrow>
  <mrow>
    <mo>(</mo>
    <munderover>
      <mo>&#x2211;<!-- ∑ --></mo>
      <mrow class="MJX-TeXAtom-ORD">
        <mi>k</mi>
        <mo>=</mo>
        <mn>1</mn>
      </mrow>
      <mi>n</mi>
    </munderover>
    <msubsup>
      <mi>b</mi>
      <mi>k</mi>
      <mn>2</mn>
    </msubsup>
    <mo>)</mo>
  </mrow>
</math>
